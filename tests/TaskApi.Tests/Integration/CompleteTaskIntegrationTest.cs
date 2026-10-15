using System.Net;
using System.Net.Http.Json;
using System.Text;
using TaskApi.Contracts;
using TaskApi.Data;

namespace TaskApi.Tests.Integration;

[Collection(InfrastructureCollection.Name)]
public sealed class CompleteTaskIntegrationTest(InfrastructureFixture infra)
{
    private static readonly TimeSpan MessageTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Create_then_complete_task_publishes_event_to_rabbitmq()
    {
        using var client = infra.Factory.CreateClient();
        await using var consumer = await RabbitTestConsumer.CreateAsync(infra.RabbitHost, infra.RabbitPort);

        // 1. create
        var created = await client.CreateTaskAsync("Buy milk", "High");
        Assert.False(created.IsCompleted);
        Assert.Null(created.CompletedAt); // CompletedAt must not be filled on creation
        Assert.Equal(Priority.High, created.Priority);

        // 2. complete
        var response = await client.PutAsync($"/tasks/{created.Id}/complete", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var completed = (await response.Content.ReadFromJsonAsync<TaskResponse>(TestJson.Options))!;
        Assert.True(completed.IsCompleted);
        Assert.NotNull(completed.CompletedAt);
        Assert.True(completed.CompletedAt >= created.CreatedAt);

        // 3. RabbitMQ received the event
        var message = await consumer.WaitForMessageAsync(created.Id, MessageTimeout);
        Assert.NotNull(message);
        Assert.Equal("Buy milk", message.Title);
        Assert.Equal(Priority.High, message.Priority);
        Assert.Equal(completed.CompletedAt!.Value.UtcDateTime, message.CompletedAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task Create_uses_defaults_IsCompleted_false_and_Priority_Medium()
    {
        using var client = infra.Factory.CreateClient();

        var created = await client.CreateTaskAsync("Defaults");

        Assert.False(created.IsCompleted);
        Assert.Equal(Priority.Medium, created.Priority);
        Assert.Null(created.CompletedAt);
    }

    [Fact]
    public async Task Parallel_complete_requests_give_exactly_one_success_and_the_rest_409()
    {
        using var client = infra.Factory.CreateClient();
        await using var consumer = await RabbitTestConsumer.CreateAsync(infra.RabbitHost, infra.RabbitPort);
        var created = await client.CreateTaskAsync("Race me");

        const int parallelism = 10;
        var responses = await Task.WhenAll(Enumerable.Range(0, parallelism)
            .Select(_ => client.PutAsync($"/tasks/{created.Id}/complete", null)));

        Assert.Equal(1, responses.Count(r => r.StatusCode == HttpStatusCode.OK));
        Assert.Equal(parallelism - 1, responses.Count(r => r.StatusCode == HttpStatusCode.Conflict));

        // ...and only the winner produced an event
        Assert.Equal(1, await consumer.CountMessagesAsync(created.Id, TimeSpan.FromSeconds(3)));
    }

    [Fact]
    public async Task Completing_an_already_completed_task_returns_409()
    {
        using var client = infra.Factory.CreateClient();
        var created = await client.CreateTaskAsync("Twice");

        Assert.Equal(HttpStatusCode.OK, (await client.PutAsync($"/tasks/{created.Id}/complete", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PutAsync($"/tasks/{created.Id}/complete", null)).StatusCode);
    }

    [Fact]
    public async Task Completing_unknown_task_returns_404()
    {
        using var client = infra.Factory.CreateClient();

        var response = await client.PutAsync($"/tasks/{Guid.NewGuid()}/complete", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Task_is_still_completed_when_rabbitmq_is_unavailable()
    {
        using var client = infra.FactoryWithoutRabbit.CreateClient();
        var created = await client.CreateTaskAsync("No broker");

        var response = await client.PutAsync($"/tasks/{created.Id}/complete", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var completed = (await response.Content.ReadFromJsonAsync<TaskResponse>(TestJson.Options))!;
        Assert.True(completed.IsCompleted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_with_empty_title_returns_400(string? title)
    {
        using var client = infra.Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/tasks", new { title });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_too_long_title_returns_400()
    {
        using var client = infra.Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/tasks", new { title = new string('a', 201) });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_missing_title_or_invalid_json_returns_400()
    {
        using var client = infra.Factory.CreateClient();

        var noTitle = await client.PostAsync("/tasks", new StringContent("{}", Encoding.UTF8, "application/json"));
        var badJson = await client.PostAsync("/tasks", new StringContent("{oops", Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, noTitle.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, badJson.StatusCode);
    }

    [Theory]
    [InlineData("{\"title\":\"x\",\"priority\":\"Urgent\"}")]
    [InlineData("{\"title\":\"x\",\"priority\":7}")]
    public async Task Create_with_invalid_priority_returns_400(string json)
    {
        using var client = infra.Factory.CreateClient();

        var response = await client.PostAsync("/tasks", new StringContent(json, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_removes_task_and_second_delete_returns_404()
    {
        using var client = infra.Factory.CreateClient();
        var created = await client.CreateTaskAsync("Delete me");

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/tasks/{created.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/tasks/{created.Id}")).StatusCode);
    }

    [Fact]
    public async Task Get_returns_pages_newest_first()
    {
        using var client = infra.Factory.CreateClient();
        var a = await client.CreateTaskAsync("page-a");
        var b = await client.CreateTaskAsync("page-b");
        var c = await client.CreateTaskAsync("page-c");

        var first = await client.GetFromJsonAsync<TaskPage>("/tasks?page=1&pageSize=2", TestJson.Options);
        var second = await client.GetFromJsonAsync<TaskPage>("/tasks?page=2&pageSize=2", TestJson.Options);

        Assert.NotNull(first);
        Assert.Equal(2, first.Items.Count);
        Assert.True(first.HasNext);
        Assert.Equal(c.Id, first.Items[0].Id);
        Assert.Equal(b.Id, first.Items[1].Id);
        Assert.Contains(second!.Items, t => t.Id == a.Id);
    }

    [Theory]
    [InlineData("/tasks?pageSize=0")]
    [InlineData("/tasks?pageSize=1000")]
    [InlineData("/tasks?page=0")]
    public async Task Get_with_invalid_paging_returns_400(string url)
    {
        using var client = infra.Factory.CreateClient();

        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync(url)).StatusCode);
    }
}
