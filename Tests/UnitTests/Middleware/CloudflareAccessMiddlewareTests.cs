using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PassXYZ.Server.Middleware;

namespace PassXYZ.Server.Tests.Middleware;

public class CloudflareAccessMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithCloudflareDisabled_ShouldPassThrough()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Cloudflare:AccessEnabled"] = "false"
            })
            .Build();

        var context = CreateHttpContext("/api/user/login");

        await TestMiddleware(context, config);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithCloudflareEnabledAndValidEmail_ShouldSetEmail()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Cloudflare:AccessEnabled"] = "true"
            })
            .Build();

        var context = CreateHttpContext("/api/user/login");
        context.Request.Headers["Cf-Access-Identity-Email"] = "test@example.com";

        await TestMiddleware(context, config);

        Assert.Equal("test@example.com", context.Items["Email"]);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithCloudflareEnabledAndNoEmail_ShouldReturnUnauthorized()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Cloudflare:AccessEnabled"] = "true"
            })
            .Build();

        var context = CreateHttpContext("/api/user/login");

        await TestMiddleware(context, config);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    private async Task TestMiddleware(HttpContext context, IConfiguration config)
    {
        var middleware = new CloudflareAccessMiddleware(next: (innerContext) =>
        {
            innerContext.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        }, config);

        await middleware.InvokeAsync(context);
    }

    private HttpContext CreateHttpContext(string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        return context;
    }
}