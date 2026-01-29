

namespace CRMUKMTPApi.Repositories
{
    public interface ITokenRepository
    {
        string FetchTokenFromContext();
        string? GetEmail();
        string? GetRole();
        List<ulong>? GetLogin();

        Task<bool> AuthenticateUser(string token);
        Task<HttpContext> SetToken(string token, HttpContext context);
    }
}