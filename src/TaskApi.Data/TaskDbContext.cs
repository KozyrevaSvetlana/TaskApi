using Microsoft.EntityFrameworkCore;

namespace TaskApi.Data;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(TaskItem.TitleMaxLength);

            // uint + IsRowVersion() => Npgsql maps it to the xmin system column:
            // no extra column, and every UPDATE is checked with "WHERE xmin = @original".
            entity.Property(x => x.Version).IsRowVersion();

            // Supports "ORDER BY created_at DESC, id DESC" used by GET /tasks without a sort step.
            entity.HasIndex(x => new { x.CreatedAt, x.Id }).IsDescending(true, true);
            //entity.HasData(TaskDataSeeder.Generate(101));
        });
    }
}
