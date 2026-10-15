using TaskApi.Data;

namespace TaskApi.Contracts;

public sealed record CreateTaskRequest(string? Title, Priority? Priority);

public sealed record TaskResponse(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt,
    Priority Priority)
{
    public static TaskResponse From(TaskItem task) =>
        new(task.Id, task.Title, task.IsCompleted, task.CreatedAt, task.CompletedAt, task.Priority);
}

public sealed record TaskPage(List<TaskResponse> Items, int Page, int PageSize, bool HasNext);
