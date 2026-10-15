namespace TaskApi.Data
{
    public static class TaskDataSeeder
    {
        private static readonly string[] Actions =
        [
            "Подготовить", "Проверить", "Обновить", "Завершить", "Изучить"
        ];

        private static readonly string[] Objects =
        [
            "отчёт", "документацию", "задачу", "план", "интеграцию"
        ];

        public static TaskItem[] Generate(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var priorities = Enum.GetValues<Priority>();

            if (count > 0 && priorities.Length == 0)
                throw new InvalidOperationException(
                    "В перечислении Priority нет значений.");

            var now = DateTimeOffset.UtcNow;
            var tasks = new TaskItem[count];

            for (var i = 0; i < count; i++)
            {
                var createdAt = now.AddMinutes(
                    -Random.Shared.Next(1, 30 * 24 * 60));

                var title =
                    $"{Actions[Random.Shared.Next(Actions.Length)]} " +
                    $"{Objects[Random.Shared.Next(Objects.Length)]} " +
                    $"#{Guid.NewGuid():N}";

                var priority = priorities[Random.Shared.Next(priorities.Length)];
                var task = TaskItem.Create(title, priority, createdAt);

                if (Random.Shared.Next(2) == 1)
                {
                    var elapsedTicks = (now - createdAt).Ticks;
                    var completedAt = createdAt.AddTicks(
                        Random.Shared.NextInt64(0, elapsedTicks + 1));

                    task.TryComplete(completedAt);
                }

                tasks[i] = task;
            }

            return tasks;
        }
    }
}
