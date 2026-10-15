using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskApi.Data;
using TaskApi.Messaging;

namespace TaskApi.Tests.Unit;

public class RabbitMqServiceTests
{
    [Fact]
    public async Task Unavailable_broker_never_throws_the_event_is_logged_and_shutdown_is_fast()
    {
        var logger = new CapturingLogger<RabbitMqService>();
        var options = Options.Create(new RabbitMqOptions
        {
            Host = "127.0.0.1",
            Port = 1, // nobody listens here
            ConnectTimeout = TimeSpan.FromSeconds(1),
            ShutdownDrainTimeout = TimeSpan.FromSeconds(3)
        });
        using var service = new RabbitMqService(options, logger);
        await service.StartAsync(CancellationToken.None);

        var message = new TaskCompletedMessage(Guid.NewGuid(), "Buy milk", DateTime.UtcNow, Priority.High);
        Assert.True(service.TryPublish(message)); // does not block, does not throw

        var stopwatch = Stopwatch.StartNew();
        await service.StopAsync(CancellationToken.None);

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(5));
        Assert.Contains(logger.Entries, e =>
            e.Level >= LogLevel.Warning && e.Message.Contains(message.TaskId.ToString()));
    }

    [Fact]
    public async Task Full_buffer_rejects_the_message_without_blocking_and_logs_it()
    {
        var logger = new CapturingLogger<RabbitMqService>();
        var options = Options.Create(new RabbitMqOptions { QueueCapacity = 1 });
        using var service = new RabbitMqService(options, logger); // not started: nothing consumes the buffer

        var first = new TaskCompletedMessage(Guid.NewGuid(), "one", DateTime.UtcNow, Priority.Low);
        var second = new TaskCompletedMessage(Guid.NewGuid(), "two", DateTime.UtcNow, Priority.Low);

        Assert.True(service.TryPublish(first));
        Assert.False(service.TryPublish(second));
        Assert.Contains(logger.Entries, e => e.Message.Contains(second.TaskId.ToString()));

        await Task.CompletedTask;
    }
}
