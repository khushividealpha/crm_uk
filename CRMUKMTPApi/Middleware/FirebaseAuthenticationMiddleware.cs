using CLIB.Constants;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Repositories;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace CRMUKMTPApi.Middleware;

public class FirebaseAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FirebaseAuthenticationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;
        if (allowAnonymous)
        {
            await _next(context);
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenRepository>();
        var token = tokenRepository.FetchTokenFromContext();
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
               context=await tokenRepository.SetToken(token, context);
              
                await _next(context);
            }
            catch(FirebaseAuthException ex)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Firebase token");
                return;
            }
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid Firebase token");
            return;
        }

    }
    //private async Task HandleWebSocketAuthentication(HttpContext context)
    //{
    //    using var scope = _serviceScopeFactory.CreateScope();
    //    var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenRepository>();

    //    // WebSocket token is typically passed in the query string or headers
    //    var token = tokenRepository.FetchTokenFromContext();

    //    if (string.IsNullOrEmpty(token))
    //    {
    //        context.Response.StatusCode = 401;
    //        await context.Response.WriteAsync("WebSocket connection requires authentication token");
    //        return;
    //    }

    //    try
    //    {
    //        var decodedToken = await FireAuthBuilder.Instance.VerifyIdTokenAsync(token);

    //        // Validate user is enabled
    //        if (!bool.Parse(decodedToken.Claims["enabled"].ToString()))
    //        {
    //            context.Response.StatusCode = 403;
    //            await context.Response.WriteAsync("User account is disabled");
    //            return;
    //        }

    //        // Store user claims for WebSocket handler to access
    //        context.Items["FirebaseUser"] = new WebSocketUser
    //        {
    //            Uid = decodedToken.Uid,
    //            Email = decodedToken.Claims["email"].ToString(),
    //            IsAdmin = bool.Parse(decodedToken.Claims["admin"].ToString()),
    //            Mt5Ids = decodedToken.Claims["mt5Ids"].ToString(),
    //            Enabled = bool.Parse(decodedToken.Claims["enabled"].ToString())
    //        };

    //        // Continue to WebSocket middleware
    //        await _next(context);
    //    }
    //    catch (Exception ex)
    //    {
    //        context.Response.StatusCode = 401;
    //        await context.Response.WriteAsync($"Invalid WebSocket token: {ex.Message}");
    //    }
    //}

}

