using Microsoft.Extensions.Logging;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Middleware;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public JwtAuthenticationMiddleware(RequestDelegate next, ILogger<JwtAuthenticationMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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
            // Development bypass: when running without Cloudflare Access, allow
            // unauthenticated requests through with a default user identity.
            if (_env.IsDevelopment())
            {
                var devUsername = context.Items["Email"] as string ?? "dev@localhost";
                context.Items["Username"] = devUsername;
                _logger.LogInformation("[Auth] Dev bypass: using username '{Username}' for path: {Path}", devUsername, path);
                await _next(context);
                return;
            }

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