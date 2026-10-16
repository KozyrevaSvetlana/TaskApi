using Microsoft.AspNetCore.Http.Features;
using System.Text.Json;
using TaskApi.Validation;


namespace TaskApi.Middleware;

/// <summary>
/// Проверяет заголовок задачи в запросе <c>POST /tasks</c> до выполнения привязки модели
/// и обращения к базе данных или зависимостям приложения.
/// Тело запроса временно буферизуется с ограничением размера, а затем перематывается,
/// чтобы endpoint мог прочитать его повторно.
/// </summary>
/// <param name="next">Следующий компонент конвейера обработки запросов.</param>
public sealed class TitleValidationMiddleware(RequestDelegate next)
{
    private const int MaxBodySize = 16 * 1024;

    /// <summary>
    /// Проверяет тело запроса на создание задачи и передаёт запрос следующему компоненту,
    /// если проверка пройдена.
    /// </summary>
    /// <param name="context">Контекст текущего HTTP-запроса.</param>
    /// <returns>Асинхронная задача обработки запроса.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsCreateTaskRequest(context.Request))
        {
            await next(context);
            return;
        }

        var maxBodySize = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (maxBodySize is { IsReadOnly: false })
            maxBodySize.MaxRequestBodySize = MaxBodySize;

        context.Request.EnableBuffering();

        Dictionary<string, string[]>? errors;
        try
        {
            errors = await ValidateBodyAsync(context.Request.Body, context.RequestAborted);
        }
        catch (BadHttpRequestException ex)
        {
            context.Response.StatusCode = ex.StatusCode; // Например, 413 — тело запроса слишком велико.
            return;
        }

        context.Request.Body.Position = 0;

        if (errors is not null)
        {
            var problem = new HttpValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Обнаружена одна или несколько ошибок проверки."
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(
                problem,
                (JsonSerializerOptions?)null,
                "application/problem+json",
                context.RequestAborted);
            return;
        }

        await next(context);
    }

    /// <summary>
    /// Определяет, является ли запрос запросом на создание задачи с JSON-телом.
    /// Запросы с другим типом содержимого передаются дальше, чтобы endpoint мог вернуть 415.
    /// </summary>
    /// <param name="request">HTTP-запрос для проверки.</param>
    /// <returns>
    /// <see langword="true"/>, если это запрос <c>POST /tasks</c> с JSON-телом;
    /// иначе <see langword="false"/>.
    /// </returns>
    private static bool IsCreateTaskRequest(HttpRequest request) =>
        HttpMethods.IsPost(request.Method)
        && request.HasJsonContentType()
        && string.Equals(
            request.Path.Value?.TrimEnd('/'),
            "/tasks",
            StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Читает JSON-тело запроса и проверяет значение свойства <c>title</c>.
    /// </summary>
    /// <param name="body">Поток с телом запроса.</param>
    /// <param name="ct">Токен отмены запроса.</param>
    /// <returns>
    /// Словарь ошибок проверки либо <see langword="null"/>, если ошибок нет.
    /// </returns>
    private static async Task<Dictionary<string, string[]>?> ValidateBodyAsync(
        Stream body,
        CancellationToken ct)
    {
        JsonDocument document;
        try
        {
            document = await JsonDocument.ParseAsync(body, cancellationToken: ct);
        }
        catch (JsonException)
        {
            return Error("body", "Тело запроса должно содержать корректный JSON.");
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return Error("body", "Тело запроса должно быть JSON-объектом.");

            string? title = null;
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (!property.Name.Equals("title", StringComparison.OrdinalIgnoreCase))
                    continue;

                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        title = property.Value.GetString();
                        break;

                    case JsonValueKind.Null:
                        break;

                    default:
                        return Error("title", "Значение поля Title должно быть строкой.");
                }
            }

            return TitleValidator.TryNormalize(title, out _, out var error)
                ? null
                : Error("title", error);
        }
    }

    /// <summary>
    /// Создаёт словарь с ошибкой проверки для указанного поля.
    /// </summary>
    /// <param name="key">Имя поля или части запроса, к которой относится ошибка.</param>
    /// <param name="message">Описание ошибки.</param>
    /// <returns>Словарь ошибок в формате, используемом ответом Validation Problem Details.</returns>
    private static Dictionary<string, string[]> Error(string key, string message) =>
        new() { [key] = [message] };
}