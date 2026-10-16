using Microsoft.EntityFrameworkCore;
using TaskApi.Contracts;
using TaskApi.Data;
using TaskApi.Messaging;

namespace TaskApi.Services;

/// <summary>
/// Результат попытки завершить задачу.
/// </summary>
public enum CompleteStatus
{
    /// <summary>
    /// Задача успешно завершена.
    /// </summary>
    Completed,

    /// <summary>
    /// Задача с указанным идентификатором не найдена.
    /// </summary>
    NotFound,

    /// <summary>
    /// Задача уже завершена или была изменена параллельным запросом.
    /// </summary>
    Conflict
}

/// <summary>
/// Содержит результат завершения задачи и данные задачи, если она была успешно завершена.
/// </summary>
/// <param name="Status">Статус операции завершения.</param>
/// <param name="Task">Данные завершённой задачи или <see langword="null"/>, если завершение не удалось.</param>
public readonly record struct CompleteResult(
    CompleteStatus Status,
    TaskResponse? Task = null);

/// <summary>
/// Предоставляет операции для создания, получения, удаления и завершения задач.
/// </summary>
/// <param name="db">Контекст базы данных задач.</param>
/// <param name="publisher">Сервис публикации событий.</param>
/// <param name="time">Поставщик текущего времени.</param>
public sealed class TaskService(
    TaskDbContext db,
    IEventPublisher publisher,
    TimeProvider time)
{
    /// <summary>
    /// Создаёт новую задачу.
    /// </summary>
    /// <param name="title">Заголовок задачи.</param>
    /// <param name="priority">Приоритет задачи.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Созданная задача.</returns>
    public async Task<TaskResponse> CreateAsync(
        string title,
        Priority priority,
        CancellationToken ct)
    {
        // CompletedAt остаётся равным null и устанавливается только при вызове TryComplete().
        var task = TaskItem.Create(title, priority, time.GetUtcNow());

        db.Tasks.Add(task);
        await db.SaveChangesAsync(ct);

        return TaskResponse.From(task);
    }

    /// <summary>
    /// Возвращает страницу задач, отсортированную по дате создания и идентификатору.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <param name="pageSize">Количество задач на странице.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Страница задач с признаком наличия следующей страницы.</returns>
    public async Task<TaskPage> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken ct)
    {
        // Только чтение: отключено отслеживание изменений, результаты сразу проецируются в DTO.
        // Загружается на одну запись больше, чтобы определить наличие следующей страницы без COUNT(*).
        var items = await db.Tasks
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)
            .Select(t => new TaskResponse(
                t.Id,
                t.Title,
                t.IsCompleted,
                t.CreatedAt,
                t.CompletedAt,
                t.Priority))
            .ToListAsync(ct);

        var hasNext = items.Count > pageSize;
        if (hasNext)
            items.RemoveAt(items.Count - 1);

        return new TaskPage(items, page, pageSize, hasNext);
    }

    /// <summary>
    /// Удаляет задачу по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// <see langword="true"/>, если задача была удалена; иначе <see langword="false"/>.
    /// </returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        // Выполняется один DELETE-запрос: SELECT не выполняется, сущность не загружается в трекер изменений.
        var deleted = await db.Tasks
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync(ct);

        return deleted > 0;
    }

    /// <summary>
    /// Пытается завершить задачу и опубликовать событие о её завершении.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Статус операции и данные задачи при успешном завершении.</returns>
    public async Task<CompleteResult> CompleteAsync(Guid id, CancellationToken ct)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (task is null)
            return new CompleteResult(CompleteStatus.NotFound);

        // Если задача уже завершена предыдущим запросом, возвращается статус конфликта.
        if (!task.TryComplete(time.GetUtcNow()))
            return new CompleteResult(CompleteStatus.Conflict);

        try
        {
            // Обновление выполняется с проверкой исходной версии записи (xmin).
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Задачу изменил другой запрос между SELECT и UPDATE — возвращается статус конфликта.
            return new CompleteResult(CompleteStatus.Conflict);
        }

        // Событие публикуется только после успешного сохранения изменений.
        // TryPublish добавляет событие в очередь в памяти и не блокирует вызывающий код.
        publisher.TryPublish(new TaskCompletedMessage(
            task.Id,
            task.Title,
            task.CompletedAt!.Value.UtcDateTime,
            task.Priority));

        return new CompleteResult(
            CompleteStatus.Completed,
            TaskResponse.From(task));
    }
}