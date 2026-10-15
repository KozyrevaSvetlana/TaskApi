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

    /// <summary>Size of the in-memory buffer between HTTP requests and the publisher loop.</summary>
    public int QueueCapacity { get; set; } = 10_000;

    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>Pause between reconnect attempts while the broker is down (messages are logged meanwhile).</summary>
    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>How long shutdown waits for already queued messages to be sent.</summary>
    public TimeSpan ShutdownDrainTimeout { get; set; } = TimeSpan.FromSeconds(5);
}
