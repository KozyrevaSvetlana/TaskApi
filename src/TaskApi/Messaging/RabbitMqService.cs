using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TaskApi.Contracts;

namespace TaskApi.Messaging;

/// <summary>
/// Fire-and-forget RabbitMQ publisher.
///  * HTTP requests only write to a bounded in-memory channel (no I/O, no waiting for confirms).
///  * A single background loop owns the connection/channel and publishes the messages.
///  * If the broker is unavailable the message is logged (with its full payload) and dropped.
///  * On shutdown the buffer is drained for at most ShutdownDrainTimeout, then channel and connection are closed.
/// </summary>
public sealed partial class RabbitMqService : BackgroundService, IEventPublisher
{
    private static readonly TimeSpan CloseTimeout = TimeSpan.FromSeconds(3);

    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqService> _logger;
    private readonly Channel<TaskCompletedMessage> _queue;
    private readonly CancellationTokenSource _abortPublishing = new();
    private readonly BasicProperties _properties = new()
    {
        ContentType = "application/json",
        DeliveryMode = DeliveryModes.Persistent
    };

    private IConnection? _connection;
    private IChannel? _channel;
    private DateTime _nextConnectAttemptUtc = DateTime.MinValue;

    public RabbitMqService(IOptions<RabbitMqOptions> options, ILogger<RabbitMqService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _queue = Channel.CreateBounded<TaskCompletedMessage>(new BoundedChannelOptions(_options.QueueCapacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait // TryWrite returns false when full instead of blocking
        });
    }

    public bool TryPublish(TaskCompletedMessage message)
    {
        if (_queue.Writer.TryWrite(message))
            return true;

        LogQueueRejected(Serialize(message));
        return false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield(); // never block host startup

        // Warm up the connection; failure is fine, publishing retries with a back-off.
        await TryEnsureChannelAsync(_abortPublishing.Token);

        try
        {
            // Ends when the writer is completed in StopAsync and the buffer is empty.
            await foreach (var message in _queue.Reader.ReadAllAsync(_abortPublishing.Token))
                await PublishAsync(message, _abortPublishing.Token);
        }
        catch (OperationCanceledException)
        {
            // drain timeout reached; leftovers are logged in StopAsync
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _queue.Writer.TryComplete(); // no new messages; the loop finishes what is already queued

        var worker = ExecuteTask;
        if (worker is not null)
        {
            try
            {
                await worker.WaitAsync(_options.ShutdownDrainTimeout, cancellationToken);
            }
            catch (TimeoutException)
            {
                LogDrainTimeout(_queue.Reader.Count);
            }
            catch (OperationCanceledException)
            {
                // host shutdown token fired
            }
            catch (Exception ex)
            {
                LogPublishFailed(ex, "worker faulted");
            }
        }

        _abortPublishing.Cancel(); // stop an in-flight publish that did not make it in time

        while (_queue.Reader.TryRead(out var left))
            LogDropped(Serialize(left));

        await DisposeConnectionAsync();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _abortPublishing.Dispose();
        base.Dispose();
    }

    private async Task PublishAsync(TaskCompletedMessage message, CancellationToken ct)
    {
        try
        {
            var channel = await TryEnsureChannelAsync(ct);
            if (channel is null)
            {
                LogDropped(Serialize(message));
                return;
            }

            var body = JsonSerializer.SerializeToUtf8Bytes(message, AppJsonContext.Default.TaskCompletedMessage);

            // No publisher confirms: we do not wait for the broker's ack.
            await channel.BasicPublishAsync(_options.Exchange, _options.RoutingKey, false, _properties, body, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            LogDropped(Serialize(message));
        }
        catch (Exception ex)
        {
            // Fail silently for the caller, but never without a trace.
            LogPublishFailed(ex, Serialize(message));
            _nextConnectAttemptUtc = DateTime.UtcNow + _options.ReconnectDelay;
            await DisposeConnectionAsync();
        }
    }

    private async Task<IChannel?> TryEnsureChannelAsync(CancellationToken ct)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        if (DateTime.UtcNow < _nextConnectAttemptUtc)
            return null;

        try
        {
            await DisposeConnectionAsync(); // drop a stale connection, if any

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(_options.ConnectTimeout);

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                ClientProvidedName = "task-api-publisher",
                RequestedConnectionTimeout = _options.ConnectTimeout,
                // Reconnection is handled here (with a back-off), not by the client library.
                AutomaticRecoveryEnabled = false,
                TopologyRecoveryEnabled = false
            };

            _connection = await factory.CreateConnectionAsync(timeout.Token);
            _channel = await _connection.CreateChannelAsync(cancellationToken: timeout.Token);
            await _channel.ExchangeDeclareAsync(
                _options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: timeout.Token);

            return _channel;
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _nextConnectAttemptUtc = DateTime.UtcNow + _options.ReconnectDelay;
            LogConnectFailed(_options.Host, _options.Port, ex.Message);
            await DisposeConnectionAsync();
            return null;
        }
    }

    private async Task DisposeConnectionAsync()
    {
        var channel = _channel;
        var connection = _connection;
        _channel = null;
        _connection = null;

        if (channel is null && connection is null)
            return;

        using var cts = new CancellationTokenSource(CloseTimeout);

        try
        {
            if (channel is { IsOpen: true })
                await channel.CloseAsync(cts.Token);
            if (channel is not null)
                await channel.DisposeAsync();
        }
        catch (Exception ex)
        {
            LogCloseFailed("channel", ex.Message);
        }

        try
        {
            if (connection is { IsOpen: true })
                await connection.CloseAsync(cts.Token);
            if (connection is not null)
                await connection.DisposeAsync();
        }
        catch (Exception ex)
        {
            LogCloseFailed("connection", ex.Message);
        }
    }

    private static string Serialize(TaskCompletedMessage message) =>
        JsonSerializer.Serialize(message, AppJsonContext.Default.TaskCompletedMessage);

    [LoggerMessage(Level = LogLevel.Warning, Message = "RabbitMQ is unavailable, event dropped: {Payload}")]
    private partial void LogDropped(string payload);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to publish event to RabbitMQ, event dropped: {Payload}")]
    private partial void LogPublishFailed(Exception exception, string payload);

    [LoggerMessage(Level = LogLevel.Error, Message = "Publish buffer is full, event dropped: {Payload}")]
    private partial void LogQueueRejected(string payload);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Cannot connect to RabbitMQ at {Host}:{Port}: {Reason}")]
    private partial void LogConnectFailed(string host, int port, string reason);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Shutdown drain timeout reached, {Remaining} event(s) were not published")]
    private partial void LogDrainTimeout(int remaining);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Error while closing RabbitMQ {Resource}: {Reason}")]
    private partial void LogCloseFailed(string resource, string reason);
}
