using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskApi.Data;

/// <summary>Used only by "dotnet ef" tooling.</summary>
public sealed class TaskDbContextFactory : IDesignTimeDbContextFactory<TaskDbContext>
{
    public TaskDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? "Host=localhost;Port=5432;Database=taskdb;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TaskDbContext(options);
    }
}
