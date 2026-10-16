using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using TaskApi.Contracts;
using TaskApi.Data;
using TaskApi.Endpoints;
using TaskApi.Filters;
using TaskApi.Messaging;
using TaskApi.Middleware;
using TaskApi.Services;

var builder = WebApplication.CreateBuilder(args);

// JSON: метаданные генерируются на этапе компиляции, перечисления передаются строками.
// Числовые значения перечислений отклоняются с ответом 400.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(allowIntegerValues: false));

    options.SerializerOptions.TypeInfoResolverChain.Insert(
        0,
        AppJsonContext.Default);
});

// Некорректные тела запросов обрабатываются с ответом 400 без выбрасывания исключения.
builder.Services.Configure<RouteHandlerOptions>(
    options => options.ThrowOnBadRequest = false);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);

// При штатной остановке фоновые службы получают время на завершение работы.
// RabbitMqService обрабатывает очередь сообщений не более 5 секунд.
builder.Services.Configure<HostOptions>(
    options => options.ShutdownTimeout = TimeSpan.FromSeconds(10));

// PostgreSQL / EF Core: контекст базы данных берётся из пула.
// Строка подключения считывается при создании контекста.
builder.Services.AddDbContextPool<TaskDbContext>((sp, options) =>
{
    var connectionString =
        sp.GetRequiredService<IConfiguration>()
            .GetConnectionString("Postgres")
        ?? throw new InvalidOperationException(
            "Не задана строка подключения ConnectionStrings:Postgres.");

    options.UseNpgsql(connectionString);
}, poolSize: 512);

// RabbitMQ: один экземпляр сервиса используется и для публикации событий,
// и в качестве фоновой службы обработки очереди.
builder.Services.AddOptions<RabbitMqOptions>()
    .BindConfiguration("RabbitMq");

builder.Services.AddSingleton<RabbitMqService>();

builder.Services.AddSingleton<IEventPublisher>(
    sp => sp.GetRequiredService<RabbitMqService>());

builder.Services.AddHostedService(
    sp => sp.GetRequiredService<RabbitMqService>());

builder.Services.AddScoped<TaskService>();

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API задач",
        Version = "v1",
        Description = "Задание от Виноградского В. Е."
    });

    var xmlPath = Path.Combine(
        AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.SchemaFilter<EnumAsStringSchemaFilter>();
});

var app = builder.Build();

app.Logger.LogInformation(
    "Среда выполнения: {Environment}",
    app.Environment.EnvironmentName);

if (string.IsNullOrWhiteSpace(
        app.Configuration.GetConnectionString("Postgres")))
{
    throw new InvalidOperationException(
        $"Не задана строка подключения ConnectionStrings:Postgres для среды " +
        $"'{app.Environment.EnvironmentName}'. Укажите её в файле " +
        $"appsettings.{{Environment}}.json или в переменной окружения " +
        "ConnectionStrings__Postgres.");
}

await app.InitializeDatabaseAsync();

app.UseExceptionHandler();
app.UseMiddleware<TitleValidationMiddleware>();

app.MapHealthChecks("/healthz");
app.MapTaskEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

/// <summary>
/// Точка входа приложения. Объявление класса позволяет использовать
/// <see cref="WebApplicationFactory{TEntryPoint}"/> в интеграционных тестах.
/// </summary>
public partial class Program
{
}