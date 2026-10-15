using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskApi.Data;
using TaskApi.Services;

namespace TaskApi.Tests.Unit;

public class TaskServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 5, 7, 12, 0, 0, TimeSpan.Zero);

    private sealed class FailingSaveInterceptor : SaveChangesInterceptor
    {
        public Exception? ExceptionToThrow { get; set; }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (ExceptionToThrow is not null)
                throw ExceptionToThrow;
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }

    private static (DbContextOptions<TaskDbContext> Options, FailingSaveInterceptor Interceptor) NewDatabase()
    {
        var interceptor = new FailingSaveInterceptor();
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;
        return (options, interceptor);
    }

    private static TaskService NewService(DbContextOptions<TaskDbContext> options, RecordingPublisher publisher) =>
        new(new TaskDbContext(options), publisher, new FixedTimeProvider(Now));

    [Fact]
    public async Task Create_uses_defaults_and_leaves_CompletedAt_empty()
    {
        var (options, _) = NewDatabase();
        var service = NewService(options, new RecordingPublisher());

        var created = await service.CreateAsync("Buy milk", Priority.Medium, CancellationToken.None);

        Assert.False(created.IsCompleted);
        Assert.Null(created.CompletedAt);
        Assert.Equal(Priority.Medium, created.Priority);
        Assert.Equal(Now, created.CreatedAt);
    }

    [Fact]
    public async Task Complete_sets_CompletedAt_and_publishes_one_message()
    {
        var (options, _) = NewDatabase();
        var publisher = new RecordingPublisher();
        var created = await NewService(options, publisher).CreateAsync("Buy milk", Priority.High, CancellationToken.None);

        var result = await NewService(options, publisher).CompleteAsync(created.Id, CancellationToken.None);

        Assert.Equal(CompleteStatus.Completed, result.Status);
        Assert.Equal(Now, result.Task!.CompletedAt);
        var message = Assert.Single(publisher.Messages);
        Assert.Equal(created.Id, message.TaskId);
        Assert.Equal("Buy milk", message.Title);
        Assert.Equal(Priority.High, message.Priority);
        Assert.Equal(Now.UtcDateTime, message.CompletedAt);
    }

    [Fact]
    public async Task Message_is_published_only_AFTER_the_task_is_saved_in_the_database()
    {
        var (options, _) = NewDatabase();
        var publisher = new RecordingPublisher();
        var created = await NewService(options, publisher).CreateAsync("Buy milk", Priority.Low, CancellationToken.None);

        var completedInDbAtPublishTime = false;
        publisher.OnPublish = _ =>
        {
            using var verify = new TaskDbContext(options);
            completedInDbAtPublishTime = verify.Tasks.Single(t => t.Id == created.Id).IsCompleted;
        };

        await NewService(options, publisher).CompleteAsync(created.Id, CancellationToken.None);

        Assert.True(completedInDbAtPublishTime);
    }

    [Fact]
    public async Task When_the_database_fails_nothing_is_published()
    {
        var (options, interceptor) = NewDatabase();
        var publisher = new RecordingPublisher();
        var created = await NewService(options, publisher).CreateAsync("Buy milk", Priority.Low, CancellationToken.None);

        interceptor.ExceptionToThrow = new InvalidOperationException("db is down");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            NewService(options, publisher).CompleteAsync(created.Id, CancellationToken.None));
        Assert.Empty(publisher.Messages);
    }

    [Fact]
    public async Task Concurrency_exception_from_the_database_is_reported_as_conflict_and_nothing_is_published()
    {
        var (options, interceptor) = NewDatabase();
        var publisher = new RecordingPublisher();
        var created = await NewService(options, publisher).CreateAsync("Buy milk", Priority.Low, CancellationToken.None);

        interceptor.ExceptionToThrow = new DbUpdateConcurrencyException("row version mismatch");

        var result = await NewService(options, publisher).CompleteAsync(created.Id, CancellationToken.None);

        Assert.Equal(CompleteStatus.Conflict, result.Status);
        Assert.Empty(publisher.Messages);
    }

    [Fact]
    public async Task Completing_twice_returns_conflict_and_publishes_only_once()
    {
        var (options, _) = NewDatabase();
        var publisher = new RecordingPublisher();
        var created = await NewService(options, publisher).CreateAsync("Buy milk", Priority.Low, CancellationToken.None);

        var first = await NewService(options, publisher).CompleteAsync(created.Id, CancellationToken.None);
        var second = await NewService(options, publisher).CompleteAsync(created.Id, CancellationToken.None);

        Assert.Equal(CompleteStatus.Completed, first.Status);
        Assert.Equal(CompleteStatus.Conflict, second.Status);
        Assert.Single(publisher.Messages);
    }

    [Fact]
    public async Task Completing_unknown_task_returns_not_found()
    {
        var (options, _) = NewDatabase();
        var publisher = new RecordingPublisher();

        var result = await NewService(options, publisher).CompleteAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(CompleteStatus.NotFound, result.Status);
        Assert.Empty(publisher.Messages);
    }
}
