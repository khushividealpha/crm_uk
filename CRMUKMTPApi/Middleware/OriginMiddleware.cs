namespace CRMUKMTPApi.Middleware;

public class OriginMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] AllowedOrigins;
    public OriginMiddleware(RequestDelegate next,IConfiguration configuration)
    {
        _next = next;
        AllowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? throw new ArgumentNullException(nameof(AllowedOrigins));
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var origin = context.Request.Headers["Origin"].ToString();
        if (string.IsNullOrEmpty(origin) || !AllowedOrigins.Contains(origin))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Origin not allowed");
            return;
        }
        //context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
        //context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        //context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        if (context.Request.Method == HttpMethods.Options)
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }
        await _next(context);
    }
}
