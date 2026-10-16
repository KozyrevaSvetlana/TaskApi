namespace TaskApi.Messaging;

public interface IEventPublisher
{
    /// <summary>
    ///Неблокирующая операция.Возвращает false, если буфер в оперативной памяти заполнен (сообщение фиксируется в логе, но отбрасывается).</summary>
    ///</summary>
    bool TryPublish(TaskCompletedMessage message);
}
