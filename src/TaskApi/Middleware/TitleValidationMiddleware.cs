using Microsoft.AspNetCore.Http.Features;
using System.Text.Json;
using TaskApi.Validation;

namespace TaskApi.Middleware;

/// <summary>
/// Rejects POST /tasks with an invalid Title before routing, model binding and any DB/DI work happen.
/// The body is buffered (size-capped) and rewound so the endpoint can still bind it.
/// </summary>
public sealed class TitleValidationMiddleware(RequestDelegate next)
{
    private const int MaxBodySize = 16 * 1024;

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
            context.Response.StatusCode = ex.StatusCode; // e.g. 413 for an oversized body
            return;
        }

        context.Request.Body.Position = 0;

        if (errors is not null)
        {
            var problem = new HttpValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(
                problem, (JsonSerializerOptions?)null, "application/problem+json", context.RequestAborted);
            return;
        }

        await next(context);
    }

    private static bool IsCreateTaskRequest(HttpRequest request) =>
        HttpMethods.IsPost(request.Method)
        && request.HasJsonContentType() // otherwise let the endpoint answer 415
        && string.Equals(request.Path.Value?.TrimEnd('/'), "/tasks", StringComparison.OrdinalIgnoreCase);

    private static async Task<Dictionary<string, string[]>?> ValidateBodyAsync(Stream body, CancellationToken ct)
    {
        JsonDocument document;
        try
        {
            document = await JsonDocument.ParseAsync(body, cancellationToken: ct);
        }
        catch (JsonException)
        {
            return Error("body", "Request body must be valid JSON.");
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return Error("body", "Request body must be a JSON object.");

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
                        return Error("title", "Title must be a string.");
                }
            }

            return TitleValidator.TryNormalize(title, out _, out var error)
                ? null
                : Error("title", error);
        }
    }

    private static Dictionary<string, string[]> Error(string key, string message) =>
        new() { [key] = [message] };
}
