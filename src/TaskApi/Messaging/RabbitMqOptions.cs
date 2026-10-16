namespace TaskApi.Messaging;

public sealed class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string Exchange { get; set; } = "task.events";
    public string RoutingKey { get; set; } = "task.completed";

    /// <summary>Размер буфера в оперативной памяти между HTTP-запросами и циклом публикации.</summary>    public int QueueCapacity { get; set; } = 10_000;
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>Пауза между попытками повторного подключения, пока брокер недоступен (в это время сообщения записываются в журнал).</summary>    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>Как долго процесс завершения работы ожидает отправки сообщений, уже поставленных в очередь.</summary>
    public TimeSpan ShutdownDrainTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Максимальное количество сообщений в очереди в памяти.
    /// </summary>
    public int QueueCapacity { get; init; } = 1_000;

    /// <summary>
    /// Задержка перед повторной попыткой подключения к RabbitMQ.
    /// </summary>
    public TimeSpan ReconnectDelay { get; init; } = TimeSpan.FromSeconds(5);
}
