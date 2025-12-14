using Auth.Domain.AppUsers.Aggregate;

namespace Auth.Infrastracture.TokenServices
{
    public interface ITokenService
    {
        //Task<TokenValidationResponse> ValidateToken(string token);
        Task<TokenPair> GenerateTokenPairAsync(AppUser user, string? ip);
        Task<TokenPair> RefreshTokenPairAsync(string accessToken, string refreshToken);
        string? GetUserIdFromAccessToken(string accessToken);
        Task<TokenPair> CreateSessionAsync(AppUser user, string? ip);
        Task<TokenPair> RefreshSessionAsync(string accessToken, string refreshToken, string? ip);
        TokenValidationResponse ValidateAccessToken(string accessToken);
        DateTime GetAccessTokenExpiry(string accessToken);
    }
}
