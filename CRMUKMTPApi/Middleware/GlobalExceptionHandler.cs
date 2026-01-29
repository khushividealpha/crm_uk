using System.Net;

namespace CRMUKMTPApi.Middleware;

public class GlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var errorId = Guid.NewGuid();
            _logger.LogError(ex, $"{errorId} : {ex.Message}");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var error = new
            {
                Id = errorId,
                Error = "Something went wrong! we are looking into resolving this."
            };
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}

