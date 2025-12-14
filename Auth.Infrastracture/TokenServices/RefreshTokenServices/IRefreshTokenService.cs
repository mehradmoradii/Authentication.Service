using Auth.Domain.AppUsers.Entity;

namespace Auth.Infrastracture.TokenServices.RefreshTokenServices
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync(Guid userId, string ipAddress);
        Task<AppRefreshToken?> GetRefreshTokenAsync(string token);
        Task<bool> ValidateRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task<List<AppRefreshToken>> GetUserTokensAsync(Guid userId);
    }
}
