namespace TaskApi.Data;

/// <summary>
/// Представляет задачу, хранящуюся в базе данных.
/// Создавать экземпляры следует с помощью метода <see cref="Create"/>,
/// чтобы сохранялись инварианты сущности.
/// </summary>
public class TaskItem
{
    /// <summary>
    /// Максимальная длина заголовка задачи в символах.
    /// </summary>
    public const int TitleMaxLength = 200;

    /// <summary>
    /// Создаёт экземпляр для Entity Framework Core.
    /// Для создания задач в приложении используйте метод <see cref="Create"/>.
    /// </summary>
    private TaskItem() { }

    /// <summary>
    /// Уникальный идентификатор задачи.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Заголовок задачи.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Показывает, завершена ли задача.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Дата и время создания задачи.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Дата и время завершения задачи.
    /// Устанавливается только при завершении задачи, а не при её создании.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// Приоритет задачи.
    /// </summary>
    public Priority Priority { get; private set; }

    /// <summary>
    /// Токен оптимистической конкурентности.
    /// В PostgreSQL отображается на системный столбец <c>xmin</c>.
    /// </summary>
    public uint Version { get; private set; }

    /// <summary>
    /// Создаёт новую задачу.
    /// </summary>
    /// <param name="title">Заголовок задачи.</param>
    /// <param name="priority">Приоритет задачи.</param>
    /// <param name="now">Дата и время создания задачи.</param>
    /// <returns>Созданная задача.</returns>
    /// <exception cref="ArgumentException">
    /// Заголовок пустой или превышает допустимую длину.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Указано недопустимое значение приоритета.
    /// </exception>
    public static TaskItem Create(
        string? title,
        Priority priority,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Заголовок не должен быть пустым.",
                nameof(title));

        var trimmed = title.Trim();
        if (trimmed.Length > TitleMaxLength)
        {
            throw new ArgumentException(
                $"Длина заголовка не должна превышать {TitleMaxLength} символов.",
                nameof(title));
        }

        if (!Enum.IsDefined(priority))
            throw new ArgumentOutOfRangeException(nameof(priority));

        return new TaskItem
        {
            Id = Guid.CreateVersion7(), // Упорядочен по времени, что удобно для индекса B-tree.
            Title = trimmed,
            IsCompleted = false,
            CreatedAt = now,
            CompletedAt = null,
            Priority = priority
        };
    }

    /// <summary>
    /// Пытается завершить задачу.
    /// </summary>
    /// <param name="now">Дата и время завершения задачи.</param>
    /// <returns>
    /// <see langword="true"/>, если задача завершена этим вызовом;
    /// <see langword="false"/>, если она уже была завершена.
    /// </returns>
    public bool TryComplete(DateTimeOffset now)
    {
        if (IsCompleted)
            return false;

        IsCompleted = true;
        CompletedAt = now;
        return true;
    }
}