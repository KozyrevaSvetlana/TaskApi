using TaskApi.Contracts;
using TaskApi.Data;
using TaskApi.Services;
using TaskApi.Validation;

namespace TaskApi.Endpoints;

public static class TaskEndpoints
{
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 100;
    private const int MaxPage = 1_000_000;

    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tasks");

        group.MapPost("/", CreateAsync);
        group.MapGet("/", GetPageAsync);
        group.MapPut("/{id:guid}/complete", CompleteAsync);
        group.MapDelete("/{id:guid}", DeleteAsync);

        return app;
    }

    private static async Task<IResult> CreateAsync(CreateTaskRequest request, TaskService service, CancellationToken ct)
    {
        // The middleware has already rejected a bad Title; this keeps the endpoint safe on its own.
        if (!TitleValidator.TryNormalize(request.Title, out var title, out var error))
            return Validation("title", error);

        var priority = request.Priority ?? Priority.Medium;
        if (!Enum.IsDefined(priority))
            return Validation("priority", "Priority must be one of: Low, Medium, High.");

        var created = await service.CreateAsync(title, priority, ct);
        return Results.Created($"/tasks/{created.Id}", created);
    }

    private static async Task<IResult> GetPageAsync(int? page, int? pageSize, TaskService service, CancellationToken ct)
    {
        var p = page ?? 1;
        var size = pageSize ?? DefaultPageSize;

        if (p < 1 || p > MaxPage)
            return Validation("page", $"Page must be between 1 and {MaxPage}.");
        if (size < 1 || size > MaxPageSize)
            return Validation("pageSize", $"PageSize must be between 1 and {MaxPageSize}.");

        return Results.Ok(await service.GetPageAsync(p, size, ct));
    }

    private static async Task<IResult> CompleteAsync(Guid id, TaskService service, CancellationToken ct)
    {
        var result = await service.CompleteAsync(id, ct);

        return result.Status switch
        {
            CompleteStatus.Completed => Results.Ok(result.Task),
            CompleteStatus.NotFound => Results.NotFound(),
            _ => Results.Problem(
                title: "Conflict",
                detail: "The task is already completed or was modified by a concurrent request.",
                statusCode: StatusCodes.Status409Conflict)
        };
    }

    private static async Task<IResult> DeleteAsync(Guid id, TaskService service, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound();

    private static IResult Validation(string field, string message) =>
        Results.ValidationProblem(new Dictionary<string, string[]> { [field] = [message] });
}
