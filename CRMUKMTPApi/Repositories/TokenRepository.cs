using Apmcrm.V1.Msgs;
using CLIB.Constants;
using CRMUKMTPApi.Helpers;
using FirebaseAdmin.Auth;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CRMUKMTPApi.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly ILogger<TokenRepository> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenRepository(ILogger<TokenRepository> logger,IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public string FetchTokenFromContext()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) == true)
        {
            var headerValue = authorizationHeader.FirstOrDefault();
            if (headerValue != null && headerValue.StartsWith("Bearer "))
            {
                return headerValue.Substring("Bearer ".Length);
            }
        }
        return string.Empty;
    }
    public string? GetEmail()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirstValue(ClaimTypes.Email);
    }
    public string? GetRole()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirstValue(ClaimTypes.Role);
    }
    public List<ulong>? GetLogin()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user == null || user.FindFirstValue("mt5Ids") == null ? new List<ulong>() : JsonConvert.DeserializeObject<List<ulong>>(user.FindFirstValue("mt5Ids"));
    }
    public string? GetVarifiedMail()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirstValue("validateEmail");
    }

    public async Task<bool> AuthenticateUser(string token)
    {
        try
        {
            var decodedToken = await FireAuthBuilder.Instance.VerifyIdTokenAsync(token);
            return true;
        }
        catch(FirebaseAuthException ex)
        {
            _logger.LogWarning(ex.Message);
            return false;
        }
    }

    public async Task<HttpContext> SetToken(string token, HttpContext context)
    {
        var decodedToken = await FireAuthBuilder.Instance.VerifyIdTokenAsync(token);
        string role = bool.Parse(decodedToken.Claims["admin"].ToString()) ? AppRoles.Admin : AppRoles.Client;
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                    new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                    new Claim(ClaimTypes.Email, decodedToken.Claims["email"].ToString()),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("enabled",decodedToken.Claims["enabled"].ToString() ),

                }, "Firebase"));
        return context;
    }
}
