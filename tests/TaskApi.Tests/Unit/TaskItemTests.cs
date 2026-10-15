using TaskApi.Data;

namespace TaskApi.Tests.Unit;

public class TaskItemTests
{
    private static readonly DateTimeOffset Now = new(2026, 5, 7, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_sets_defaults_and_does_not_fill_CompletedAt()
    {
        var task = TaskItem.Create("Buy milk", Priority.Medium, Now);

        Assert.False(task.IsCompleted);
        Assert.Null(task.CompletedAt); // CompletedAt must NOT be set on creation
        Assert.Equal(Now, task.CreatedAt);
        Assert.Equal(Priority.Medium, task.Priority);
        Assert.NotEqual(Guid.Empty, task.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void Create_rejects_empty_or_whitespace_title(string? title)
    {
        Assert.Throws<ArgumentException>(() => TaskItem.Create(title, Priority.Low, Now));
    }

    [Fact]
    public void Create_rejects_title_longer_than_200_chars()
    {
        Assert.Throws<ArgumentException>(() => TaskItem.Create(new string('a', 201), Priority.Low, Now));
        Assert.NotNull(TaskItem.Create(new string('a', 200), Priority.Low, Now));
    }

    [Fact]
    public void TryComplete_sets_CompletedAt_only_at_completion()
    {
        var task = TaskItem.Create("Buy milk", Priority.High, Now);
        var completedAt = Now.AddMinutes(5);

        Assert.True(task.TryComplete(completedAt));

        Assert.True(task.IsCompleted);
        Assert.Equal(completedAt, task.CompletedAt);
    }

    [Fact]
    public void TryComplete_second_call_returns_false_and_keeps_original_CompletedAt()
    {
        var task = TaskItem.Create("Buy milk", Priority.High, Now);
        task.TryComplete(Now.AddMinutes(1));

        Assert.False(task.TryComplete(Now.AddMinutes(2)));
        Assert.Equal(Now.AddMinutes(1), task.CompletedAt);
    }
}
