using Microsoft.EntityFrameworkCore;
using TaskApi.Contracts;
using TaskApi.Data;
using TaskApi.Messaging;

namespace TaskApi.Services;

public enum CompleteStatus
{
    Completed,
    NotFound,
    Conflict
}

public readonly record struct CompleteResult(CompleteStatus Status, TaskResponse? Task = null);

public sealed class TaskService(TaskDbContext db, IEventPublisher publisher, TimeProvider time)
{
    public async Task<TaskResponse> CreateAsync(string title, Priority priority, CancellationToken ct)
    {
        // CompletedAt stays null: it is set only by TryComplete().
        var task = TaskItem.Create(title, priority, time.GetUtcNow());

        db.Tasks.Add(task);
        await db.SaveChangesAsync(ct);

        return TaskResponse.From(task);
    }

    public async Task<TaskPage> GetPageAsync(int page, int pageSize, CancellationToken ct)
    {
        // Read-only, no change tracking, projection straight into the DTO,
        // pageSize + 1 rows to know whether a next page exists (no COUNT(*)).
        var items = await db.Tasks
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)
            .Select(t => new TaskResponse(t.Id, t.Title, t.IsCompleted, t.CreatedAt, t.CompletedAt, t.Priority))
            .ToListAsync(ct);

        var hasNext = items.Count > pageSize;
        if (hasNext)
            items.RemoveAt(items.Count - 1);

        return new TaskPage(items, page, pageSize, hasNext);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        // A single DELETE statement: no SELECT, nothing is loaded into the change tracker.
        var deleted = await db.Tasks.Where(t => t.Id == id).ExecuteDeleteAsync(ct);
        return deleted > 0;
    }

    public async Task<CompleteResult> CompleteAsync(Guid id, CancellationToken ct)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (task is null)
            return new CompleteResult(CompleteStatus.NotFound);

        // Already completed by an earlier request => 409.
        if (!task.TryComplete(time.GetUtcNow()))
            return new CompleteResult(CompleteStatus.Conflict);

        try
        {
            // UPDATE ... WHERE id = @id AND xmin = @originalVersion
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Somebody else completed the task between our SELECT and UPDATE => 409.
            return new CompleteResult(CompleteStatus.Conflict);
        }

        // Publish strictly AFTER the successful commit, so a failed save never produces an event.
        // TryPublish only enqueues in memory: it neither blocks nor throws.
        publisher.TryPublish(new TaskCompletedMessage(
            task.Id, task.Title, task.CompletedAt!.Value.UtcDateTime, task.Priority));

        return new CompleteResult(CompleteStatus.Completed, TaskResponse.From(task));
    }
}
