using System.Text;
using Microsoft.AspNetCore.Http;
using TaskApi.Middleware;

namespace TaskApi.Tests.Unit;

public class TitleValidationMiddlewareTests
{
    private static DefaultHttpContext CreateContext(string json, string method = "POST", string path = "/tasks")
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        context.Request.ContentType = "application/json";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
        context.Response.Body = new MemoryStream();
        return context;
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"title\":null}")]
    [InlineData("{\"title\":\"\"}")]
    [InlineData("{\"title\":\"   \"}")]
    [InlineData("{\"title\":123}")]
    [InlineData("not json")]
    [InlineData("[]")]
    public async Task Invalid_title_is_rejected_with_400_without_calling_next(string json)
    {
        var nextCalled = false;
        var middleware = new TitleValidationMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateContext(json);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.False(nextCalled);
    }

    [Fact]
    public async Task Title_longer_than_200_is_rejected()
    {
        var middleware = new TitleValidationMiddleware(_ => Task.CompletedTask);
        var context = CreateContext($"{{\"title\":\"{new string('a', 201)}\"}}");

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task Valid_title_passes_through_and_body_can_still_be_read()
    {
        const string json = "{\"title\":\"Buy milk\",\"priority\":\"High\"}";
        string? bodySeenByEndpoint = null;
        var middleware = new TitleValidationMiddleware(async ctx =>
        {
            using var reader = new StreamReader(ctx.Request.Body);
            bodySeenByEndpoint = await reader.ReadToEndAsync();
        });
        var context = CreateContext(json);

        await middleware.InvokeAsync(context);

        Assert.Equal(json, bodySeenByEndpoint);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Theory]
    [InlineData("GET", "/tasks")]
    [InlineData("PUT", "/tasks/3fa85f64-5717-4562-b3fc-2c963f66afa6/complete")]
    [InlineData("POST", "/other")]
    public async Task Other_requests_are_ignored(string method, string path)
    {
        var nextCalled = false;
        var middleware = new TitleValidationMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });

        await middleware.InvokeAsync(CreateContext("{}", method, path));

        Assert.True(nextCalled);
    }
}
