using Microsoft.Extensions.Logging;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Middleware;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;

    public JwtAuthenticationMiddleware(RequestDelegate next, ILogger<JwtAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        var path = context.Request.Path;
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var allowAnonymousPaths = new[] { "/api/user/login", "/api/user/signup", "/api/apps.json", "/api/agents.json", "/api/widgets.json" };
        if (allowAnonymousPaths.Any(p => context.Request.Path.StartsWithSegments(p)))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("[Auth] No token provided for path: {Path} from {IpAddress}", path, ipAddress);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var username = jwtService.ValidateToken(token);
        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("[Auth] Invalid token for path: {Path} from {IpAddress}", path, ipAddress);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Items["Username"] = username;
        await _next(context);
    }
}