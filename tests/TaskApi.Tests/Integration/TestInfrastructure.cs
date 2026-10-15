using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskApi.Contracts;
using TaskApi.Messaging;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace TaskApi.Tests.Integration;

internal static class TestJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}

internal static class ApiClientExtensions
{
    public static async Task<TaskResponse> CreateTaskAsync(this HttpClient client, string title, string? priority = null)
    {
        var response = await client.PostAsJsonAsync("/tasks", new { title, priority });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<TaskResponse>(TestJson.Options))!;
    }
}

internal static class RabbitCredentials
{
    public const string UserName = "test";
    public const string Password = "test";
}

/// <summary>Real API (in-memory TestServer) wired to real PostgreSQL and RabbitMQ containers.</summary>
public sealed class ApiFactory(string postgresConnectionString, string rabbitHost, int rabbitPort)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres"] = postgresConnectionString,
            ["RabbitMq:Host"] = rabbitHost,
            ["RabbitMq:Port"] = rabbitPort.ToString(),
            ["RabbitMq:UserName"] = RabbitCredentials.UserName,   // новое
            ["RabbitMq:Password"] = RabbitCredentials.Password,   // новое
            ["RabbitMq:ConnectTimeout"] = "00:00:02"
        }));
    }
}

public sealed class InfrastructureFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().WithImage("postgres:17-alpine").Build();
    private readonly RabbitMqContainer _rabbit = new RabbitMqBuilder()
        .WithImage("rabbitmq:4-alpine")
        .WithUsername(RabbitCredentials.UserName)
        .WithPassword(RabbitCredentials.Password)
        .Build();

    public ApiFactory Factory { get; private set; } = null!;

    /// <summary>The same API, but pointed at a RabbitMQ port nobody listens on.</summary>
    public ApiFactory FactoryWithoutRabbit { get; private set; } = null!;

    public string RabbitHost => _rabbit.Hostname;
    public int RabbitPort => _rabbit.GetMappedPublicPort(5672);

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_postgres.StartAsync(), _rabbit.StartAsync());

        var connectionString = _postgres.GetConnectionString();
        Factory = new ApiFactory(connectionString, RabbitHost, RabbitPort);
        FactoryWithoutRabbit = new ApiFactory(connectionString, "127.0.0.1", 1);
    }

    public async Task DisposeAsync()
    {
        await Factory.DisposeAsync();
        await FactoryWithoutRabbit.DisposeAsync();
        await _postgres.DisposeAsync();
        await _rabbit.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class InfrastructureCollection : ICollectionFixture<InfrastructureFixture>
{
    public const string Name = "infrastructure";
}

/// <summary>Binds its own exclusive queue to task.events / task.completed and reads what the API publishes.</summary>
public sealed class RabbitTestConsumer : IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queue;

    private RabbitTestConsumer(IConnection connection, IChannel channel, string queue)
    {
        _connection = connection;
        _channel = channel;
        _queue = queue;
    }

    public static async Task<RabbitTestConsumer> CreateAsync(string host, int port)
    {
        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = RabbitCredentials.UserName,
            Password = RabbitCredentials.Password
        };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        // Same arguments as the API uses, so the declaration is idempotent whoever runs first.
        await channel.ExchangeDeclareAsync("task.events", ExchangeType.Topic, durable: true, autoDelete: false);
        var queue = await channel.QueueDeclareAsync(queue: string.Empty, durable: false, exclusive: true, autoDelete: true);
        await channel.QueueBindAsync(queue.QueueName, "task.events", "task.completed");

        return new RabbitTestConsumer(connection, channel, queue.QueueName);
    }

    public async Task<TaskCompletedMessage?> WaitForMessageAsync(Guid taskId, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var result = await _channel.BasicGetAsync(_queue, autoAck: true);
            if (result is null)
            {
                await Task.Delay(50);
                continue;
            }

            var message = JsonSerializer.Deserialize<TaskCompletedMessage>(result.Body.Span, TestJson.Options);
            if (message?.TaskId == taskId)
                return message;
        }

        return null;
    }

    public async Task<int> CountMessagesAsync(Guid taskId, TimeSpan window)
    {
        var count = 0;
        var deadline = DateTime.UtcNow + window;
        while (DateTime.UtcNow < deadline)
        {
            var result = await _channel.BasicGetAsync(_queue, autoAck: true);
            if (result is null)
            {
                await Task.Delay(50);
                continue;
            }

            var message = JsonSerializer.Deserialize<TaskCompletedMessage>(result.Body.Span, TestJson.Options);
            if (message?.TaskId == taskId)
                count++;
        }

        return count;
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
