using Auth.Domain.AppUsers.Aggregate;
using System.Security.Claims;

namespace Auth.Infrastracture.TokenServices.AccessTokenServices
{
    public interface IAccessTokenService
    {
        Task<string> GenerateAccessToken(AppUser user);
        TokenValidationResponse ValidateAccessToken(string token);
        string? GetUserIdFromToken(string token);
        string? GetEmailFromToken(string token);
        List<string> GetRolesFromToken(string token);
        DateTime GetExpirationFromToken(string token);
    }
}
