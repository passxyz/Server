using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PassXYZ.Server.Middleware;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Tests.Middleware;

public class JwtAuthenticationMiddlewareTests
{
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly IConfiguration _config;
    private readonly Mock<ILogger<JwtAuthenticationMiddleware>> _mockLogger;

    public JwtAuthenticationMiddlewareTests()
    {
        _mockJwtService = new Mock<IJwtService>();
        _config = new ConfigurationBuilder().Build();
        _mockLogger = new Mock<ILogger<JwtAuthenticationMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_WithAnonymousPath_ShouldPassThrough()
    {
        var context = CreateHttpContext("/api/user/login");
        _mockJwtService.Setup(s => s.ValidateToken(It.IsAny<string>())).Returns("testuser");

        await TestMiddleware(context);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithValidToken_ShouldSetUsername()
    {
        var context = CreateHttpContext("/api/vault/groups/root/items");
        context.Request.Headers["Authorization"] = "Bearer valid-token";
        _mockJwtService.Setup(s => s.ValidateToken("valid-token")).Returns("testuser");

        await TestMiddleware(context);

        Assert.Equal("testuser", context.Items["Username"]);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidToken_ShouldReturnUnauthorized()
    {
        var context = CreateHttpContext("/api/vault/groups/root/items");
        context.Request.Headers["Authorization"] = "Bearer invalid-token";
        _mockJwtService.Setup(s => s.ValidateToken("invalid-token")).Returns(string.Empty);

        await TestMiddleware(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithNoToken_ShouldReturnUnauthorized()
    {
        var context = CreateHttpContext("/api/vault/groups/root/items");

        await TestMiddleware(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    private async Task TestMiddleware(HttpContext context)
    {
        var middleware = new JwtAuthenticationMiddleware(next: (innerContext) =>
        {
            innerContext.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        }, _mockLogger.Object);

        await middleware.InvokeAsync(context, _mockJwtService.Object);
    }

    private HttpContext CreateHttpContext(string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        return context;
    }
}