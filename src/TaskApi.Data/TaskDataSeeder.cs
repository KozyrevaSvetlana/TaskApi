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

            var random = new Random(101);
            var now = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var tasks = new TaskItem[count];

            for (var i = 0; i < count; i++)
            {
                var createdAt = now.AddMinutes(
                    -random.Next(1, 30 * 24 * 60));

                var title =
                    $"{Actions[random.Next(Actions.Length)]} " +
                    $"{Objects[random.Next(Objects.Length)]} " +
                    $"#{i + 1:D3}";

                var priority = priorities[random.Next(priorities.Length)];

                // ID стабилен между запусками генератора.
                var id = new Guid(i + 1, 0, 0, new byte[8]);

                var task = CreateSeeded(id, title, priority, createdAt);

                if (random.Next(2) == 1)
                {
                    var elapsedTicks = (now - createdAt).Ticks;
                    var completedAt = createdAt.AddTicks(
                        random.NextInt64(0, elapsedTicks + 1));

                    task.TryComplete(completedAt);
                }

                tasks[i] = task;
            }

            return tasks;
        }

        internal static TaskItem CreateSeeded(
            Guid id,
            string title,
            Priority priority,
            DateTimeOffset createdAt)
        {
            var task = TaskItem.Create(title, priority, createdAt);
            task.Id = id;
            return task;
        }
    }
}
