using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskApi.Data;

namespace TaskApi.Services;

/// <summary>
/// Инициализирует базу данных: применяет миграции, если они есть,
/// или создаёт схему на основе модели.
/// При временных ошибках подключения повторяет попытку, так как сервер базы данных
/// может ещё запускаться.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Инициализирует базу данных и её схему.
    /// Если в проекте есть миграции, применяет их; в противном случае
    /// создаёт схему на основе модели Entity Framework Core.
    /// При временных ошибках подключения повторяет попытки,
    /// поскольку сервер базы данных может ещё запускаться.
    /// </summary>
    /// <param name="app">Веб-приложение, из контейнера зависимостей которого берётся контекст базы данных.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Асинхронная задача инициализации базы данных.</returns>
    public static async Task InitializeDatabaseAsync(
        this WebApplication app,
        CancellationToken ct = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                if (db.Database.GetMigrations().Any())
                    await db.Database.MigrateAsync(ct);
                else
                    await db.Database.EnsureCreatedAsync(ct);

                return;
            }
            catch (Exception ex)
                when (attempt < 10 && !ct.IsCancellationRequested && IsTransient(ex))
            {
                app.Logger.LogWarning(
                    ex,
                    "База данных ещё не готова (попытка {Attempt} из 10). Повторная попытка...");
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }
    }

    /// <summary>
    /// Определяет, является ли ошибка временной и подходящей для повторной попытки.
    /// Повторяются только ошибки подключения; например, неверный пароль или отсутствие
    /// базы данных не должны приводить к ожиданию перед сообщением об ошибке.
    /// </summary>
    /// <param name="ex">Исключение для проверки.</param>
    /// <returns>
    /// <see langword="true"/>, если ошибка связана с временной проблемой подключения;
    /// иначе <see langword="false"/>.
    /// </returns>
    private static bool IsTransient(Exception ex) =>
        ex is NpgsqlException { IsTransient: true }
        || ex.InnerException is NpgsqlException { IsTransient: true };
}