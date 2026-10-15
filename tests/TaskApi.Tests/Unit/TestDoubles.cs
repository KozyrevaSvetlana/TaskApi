using Microsoft.Extensions.Logging;
using TaskApi.Messaging;

namespace TaskApi.Tests.Unit;

internal sealed class RecordingPublisher : IEventPublisher
{
    public List<TaskCompletedMessage> Messages { get; } = [];
    public Action<TaskCompletedMessage>? OnPublish { get; set; }

    public bool TryPublish(TaskCompletedMessage message)
    {
        OnPublish?.Invoke(message);
        Messages.Add(message);
        return true;
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}

internal sealed class CapturingLogger<T> : ILogger<T>
{
    public List<(LogLevel Level, string Message)> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        lock (Entries)
            Entries.Add((logLevel, formatter(state, exception)));
    }
}
