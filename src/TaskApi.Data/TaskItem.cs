namespace TaskApi.Data;

public class TaskItem
{
    public const int TitleMaxLength = 200;

    // For EF Core only. Instances are created through Create(), so invariants always hold.
    private TaskItem() { }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public bool IsCompleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Set only at the moment of completion (never on creation).</summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    public Priority Priority { get; private set; }

    /// <summary>Optimistic concurrency token. In PostgreSQL it maps to the xmin system column.</summary>
    public uint Version { get; private set; }

    public static TaskItem Create(string? title, Priority priority, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must not be empty.", nameof(title));

        var trimmed = title.Trim();
        if (trimmed.Length > TitleMaxLength)
            throw new ArgumentException($"Title must not exceed {TitleMaxLength} characters.", nameof(title));

        if (!Enum.IsDefined(priority))
            throw new ArgumentOutOfRangeException(nameof(priority));

        return new TaskItem
        {
            Id = Guid.CreateVersion7(), // time-ordered => friendly to the B-tree index
            Title = trimmed,
            IsCompleted = false,
            CreatedAt = now,
            CompletedAt = null,
            Priority = priority
        };
    }

    /// <returns>false if the task was already completed.</returns>
    public bool TryComplete(DateTimeOffset now)
    {
        if (IsCompleted)
            return false;

        IsCompleted = true;
        CompletedAt = now;
        return true;
    }
}
