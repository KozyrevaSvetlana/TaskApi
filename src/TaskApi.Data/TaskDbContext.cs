using Microsoft.EntityFrameworkCore;

namespace TaskApi.Data;

/// <summary>
/// Контекст Entity Framework Core для доступа к задачам в базе данных.
/// </summary>
/// <param name="options">Параметры настройки контекста базы данных.</param>
public class TaskDbContext(DbContextOptions<TaskDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// Набор задач, хранящихся в базе данных.
    /// </summary>
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    /// <summary>
    /// Настраивает отображение сущностей и их свойств на таблицы и столбцы базы данных.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели Entity Framework Core.</param>
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

            // Тип uint вместе с IsRowVersion() отображается провайдером Npgsql
            // на системный столбец xmin. Отдельный столбец не создаётся,
            // а при обновлении проверяется исходная версия записи.
            entity.Property(x => x.Version).IsRowVersion();

            // Индекс поддерживает сортировку по CreatedAt и Id по убыванию,
            // используемую при получении списка задач.
            entity.HasIndex(x => new { x.CreatedAt, x.Id })
                .IsDescending(true, true);

            // При необходимости можно включить начальное заполнение данных:
            // entity.HasData(TaskDataSeeder.Generate(101));
        });
    }
}