using TaskApi.Contracts;
using TaskApi.Data;
using TaskApi.Services;
using TaskApi.Validation;

namespace TaskApi.Endpoints;

/// <summary>
/// Регистрирует HTTP-маршруты для работы с задачами.
/// </summary>
public static class TaskEndpoints
{
    /// <summary>
    /// Размер страницы по умолчанию при получении списка задач.
    /// </summary>
    private const int DefaultPageSize = 50;

    /// <summary>
    /// Максимально допустимое количество задач на странице.
    /// </summary>
    private const int MaxPageSize = 100;

    /// <summary>
    /// Максимально допустимый номер страницы.
    /// </summary>
    private const int MaxPage = 1_000_000;

    /// <summary>
    /// Добавляет маршруты для создания, получения, завершения и удаления задач.
    /// </summary>
    /// <param name="app">Построитель маршрутов приложения.</param>
    /// <returns>Тот же построитель маршрутов для дальнейшей настройки.</returns>
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tasks");

        group.MapPost("/", CreateAsync);
        group.MapGet("/", GetPageAsync);
        group.MapPut("/{id:guid}/complete", CompleteAsync);
        group.MapDelete("/{id:guid}", DeleteAsync);

        return app;
    }

    /// <summary>
    /// Создаёт новую задачу.
    /// </summary>
    /// <param name="request">Данные для создания задачи.</param>
    /// <param name="service">Сервис для работы с задачами.</param>
    /// <param name="ct">Токен отмены запроса.</param>
    /// <returns>
    /// Ответ с созданной задачей и статусом 201 либо ошибкой валидации.
    /// </returns>
    private static async Task<IResult> CreateAsync(
        CreateTaskRequest request,
        TaskService service,
        CancellationToken ct)
    {
        // Middleware уже проверил заголовок задачи, но endpoint сохраняет самостоятельную проверку.
        if (!TitleValidator.TryNormalize(request.Title, out var title, out var error))
            return Validation("title", error);

        var priority = request.Priority ?? Priority.Medium;
        if (!Enum.IsDefined(priority))
            return Validation("priority", "Priority должен иметь значения: Low, Medium, High.");

        var created = await service.CreateAsync(title, priority, ct);
        return Results.Created($"/tasks/{created.Id}", created);
    }

    /// <summary>
    /// Возвращает страницу со списком задач.
    /// </summary>
    /// <param name="page">Номер страницы; если не задан, используется первая страница.</param>
    /// <param name="pageSize">Размер страницы; если не задан, используется значение по умолчанию.</param>
    /// <param name="service">Сервис для получения задач.</param>
    /// <param name="ct">Токен отмены запроса.</param>
    /// <returns>
    /// Ответ со страницей задач либо ошибкой валидации параметров.
    /// </returns>
    private static async Task<IResult> GetPageAsync(
        int? page,
        int? pageSize,
        TaskService service,
        CancellationToken ct)
    {
        var p = page ?? 1;
        var size = pageSize ?? DefaultPageSize;

        if (p < 1 || p > MaxPage)
            return Validation("page", $"Страница должна быть 1 до {MaxPage}.");

        if (size < 1 || size > MaxPageSize)
            return Validation("pageSize", $"PageSize должен быть между 1 и {MaxPageSize}.");

        return Results.Ok(await service.GetPageAsync(p, size, ct));
    }

    /// <summary>
    /// Помечает задачу как завершённую.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="service">Сервис для работы с задачами.</param>
    /// <param name="ct">Токен отмены запроса.</param>
    /// <returns>
    /// Ответ с обновлённой задачей, статусом 404, если задача не найдена,
    /// или статусом 409 при конфликте.
    /// </returns>
    private static async Task<IResult> CompleteAsync(
        Guid id,
        TaskService service,
        CancellationToken ct)
    {
        var result = await service.CompleteAsync(id, ct);

        return result.Status switch
        {
            CompleteStatus.Completed => Results.Ok(result.Task),
            CompleteStatus.NotFound => Results.NotFound(),
            _ => Results.Problem(
                title: "Conflict",
                detail: "Задача уже выполнена или была изменена в результате параллельного запроса.",
                statusCode: StatusCodes.Status409Conflict)
        };
    }

    /// <summary>
    /// Удаляет задачу.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="service">Сервис для работы с задачами.</param>
    /// <param name="ct">Токен отмены запроса.</param>
    /// <returns>
    /// Ответ со статусом 204, если задача удалена, или 404, если она не найдена.
    /// </returns>
    private static async Task<IResult> DeleteAsync(
        Guid id,
        TaskService service,
        CancellationToken ct) =>
        await service.DeleteAsync(id, ct)
            ? Results.NoContent()
            : Results.NotFound();

    /// <summary>
    /// Формирует ответ с ошибкой валидации для указанного поля.
    /// </summary>
    /// <param name="field">Имя поля, для которого обнаружена ошибка.</param>
    /// <param name="message">Описание ошибки.</param>
    /// <returns>Ответ в формате Validation Problem Details.</returns>
    private static IResult Validation(string field, string message) =>
        Results.ValidationProblem(new Dictionary<string, string[]> { [field] = [message] });
}