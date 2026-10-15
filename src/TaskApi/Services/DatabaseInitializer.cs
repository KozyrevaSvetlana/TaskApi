using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskApi.Data;

namespace TaskApi.Services;

/// <summary>
/// Applies migrations if the Data project has any, otherwise creates the schema from the model.
/// Retries a few times because the database container may still be starting.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Creates the database and schema (applies migrations if any, otherwise builds the schema from the model).
    /// Retries transient connection errors because the database server may still be starting.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication app, CancellationToken ct = default)
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
            catch (Exception ex) when (attempt < 10 && !ct.IsCancellationRequested && IsTransient(ex))
            {
                app.Logger.LogWarning(ex, "Database is not ready (attempt {Attempt}/10), retrying...", attempt);
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }
    }

    // Retry only temporary failures (server is starting, network); a wrong password or a missing
    // database should fail immediately with the real error instead of waiting 20 seconds.
    private static bool IsTransient(Exception ex) =>
        ex is NpgsqlException { IsTransient: true } || ex.InnerException is NpgsqlException { IsTransient: true };
}
