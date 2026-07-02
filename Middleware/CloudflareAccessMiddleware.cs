namespace PassXYZ.Server.Middleware;

public class CloudflareAccessMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public CloudflareAccessMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var isCloudflareAccessEnabled = bool.TryParse(_configuration["Cloudflare:AccessEnabled"], out var enabled) && enabled;
        
        if (!isCloudflareAccessEnabled)
        {
            await _next(context);
            return;
        }

        var email = context.Request.Headers["Cf-Access-Identity-Email"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(email))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Items["Email"] = email;
        await _next(context);
    }
}