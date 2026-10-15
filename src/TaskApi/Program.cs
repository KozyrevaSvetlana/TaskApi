using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskApi.Contracts;
using TaskApi.Data;
using TaskApi.Endpoints;
using TaskApi.Messaging;
using TaskApi.Middleware;
using TaskApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ---- JSON: source-generated metadata, enums as strings (numbers are rejected => 400) ----
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

// Bad request bodies are answered with 400 directly instead of throwing.
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);

// ---- Graceful shutdown: give hosted services time to flush (RabbitMqService drains for up to 5s) ----
builder.Services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(10));

// ---- PostgreSQL / EF Core: pooled DbContext, connection string is read lazily (test friendly) ----
builder.Services.AddDbContextPool<TaskDbContext>((sp, options) =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Postgres")
        ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured.");

    options.UseNpgsql(connectionString);
}, poolSize: 512);

// ---- RabbitMQ: one singleton that is both the publisher and the hosted background loop ----
builder.Services.AddOptions<RabbitMqOptions>().BindConfiguration("RabbitMq");
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSingleton<IEventPublisher>(sp => sp.GetRequiredService<RabbitMqService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<RabbitMqService>());

builder.Services.AddScoped<TaskService>();

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseExceptionHandler();
app.UseMiddleware<TitleValidationMiddleware>();

app.MapGet("/healthz", () => Results.Ok());
app.MapTaskEndpoints();

app.Run();

// Needed by WebApplicationFactory<Program> in the integration tests.
public partial class Program
{
}
