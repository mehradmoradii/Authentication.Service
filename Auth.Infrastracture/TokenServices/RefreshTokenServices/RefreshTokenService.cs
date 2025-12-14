using Auth.Domain.AppUsers.Entity;
using StackExchange.Redis;
using System.Text.Json;

namespace Auth.Infrastracture.TokenServices.RefreshTokenServices
{
    public class RefreshTokenService : IRefreshTokenService
    {
    
        private readonly IDatabase _db;
        public RefreshTokenService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        private string GetUserKey(Guid userId) => $"user:{userId}:tokens";
        private string GetTokenKey(string token) => $"token:{token}";

        public async Task<string> GenerateRefreshTokenAsync(Guid userId, string ipAddress)
        {
            var token = Guid.NewGuid().ToString();
            var refreshToken = new AppRefreshToken
            {
                Token = token,
                Expiration = DateTime.Now.AddDays(7),
                UserId = userId
            };
            var tokenJson = JsonSerializer.Serialize(refreshToken);
            // Store token details
            await _db.StringSetAsync(GetTokenKey(token), tokenJson, TimeSpan.FromDays(7));
            // Add token to user's set
            await _db.SetAddAsync(GetUserKey(userId), token);
            return tokenJson;

        }

        public async Task<AppRefreshToken?> GetRefreshTokenAsync(string token)
        {
            var tokenJson = await _db.StringGetAsync(GetTokenKey(token));
            if (tokenJson.IsNullOrEmpty) return null;

            return JsonSerializer.Deserialize<AppRefreshToken>(tokenJson);
        }

        public async Task<List<AppRefreshToken>> GetUserTokensAsync(Guid userId)
        {
            var tokens = await _db.SetMembersAsync(GetUserKey(userId));
            var refreshTokens = new List<AppRefreshToken>();
            foreach (var item in tokens)
            {
                var tokenJson = await _db.StringGetAsync(GetTokenKey(item));
                if (!tokenJson.IsNullOrEmpty)
                {
                    var refreshToken = JsonSerializer.Deserialize<AppRefreshToken>(tokenJson);
                    if (refreshToken != null)
                    {
                        refreshTokens.Add(refreshToken);
                    }
                }
            }
            return refreshTokens;
        }

        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            var tokens = await _db.SetMembersAsync(GetUserKey(userId));
            foreach(var item in tokens)
            {
                await _db.KeyDeleteAsync(GetTokenKey(item));
            }
            await _db.KeyDeleteAsync(GetUserKey(userId));
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await GetRefreshTokenAsync(token);
            if (refreshToken == null) return;
            await _db.KeyDeleteAsync(GetTokenKey(token));
            await _db.SetRemoveAsync(GetUserKey(refreshToken.UserId), token);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token)
        {
            var refreshToken = await GetRefreshTokenAsync(token);
            if (refreshToken == null) return false;
            return refreshToken.Expiration > DateTime.Now;
        }
    }
}
