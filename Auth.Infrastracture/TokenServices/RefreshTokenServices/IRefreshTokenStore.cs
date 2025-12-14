using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastracture.TokenServices.RefreshTokenServices
{
    public interface IRefreshTokenStore
    {
        Task SaveAsync(Guid userId, string refreshToken, string? ip, DateTime expiresAt);
        Task<RefreshTokenRecord?> GetAsync(string refreshToken);
        Task RevokeAsync(string refreshToken);
        Task RevokeAllForUserAsync(Guid userId);
    }

}
