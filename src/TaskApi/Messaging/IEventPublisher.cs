namespace TaskApi.Messaging;

public interface IEventPublisher
{
    /// <summary>Non-blocking. Returns false if the in-memory buffer is full (the message is logged and dropped).</summary>
    bool TryPublish(TaskCompletedMessage message);
}
