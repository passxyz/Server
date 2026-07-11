using PassXYZ.Server.Services;

namespace PassXYZ.Server.Middleware;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public JwtAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        var allowAnonymousPaths = new[] { "/api/user/login", "/api/user/signup", "/api/apps.json", "/api/agents.json", "/api/widgets.json" };
        if (allowAnonymousPaths.Any(p => context.Request.Path.StartsWithSegments(p)))
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var username = jwtService.ValidateToken(token);
        if (string.IsNullOrEmpty(username))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Items["Username"] = username;
        await _next(context);
    }
}