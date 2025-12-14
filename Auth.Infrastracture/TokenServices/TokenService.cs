using Auth.Domain.AppUsers.Aggregate;
using Auth.Infrastracture.TokenServices.AccessTokenServices;
using Auth.Infrastracture.TokenServices.RefreshTokenServices;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens.Experimental;


namespace Auth.Infrastracture.TokenServices
{
  
    public class TokenService : ITokenService
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _cfg;
        

        

        public TokenService(
            IAccessTokenService accessToken,
            IRefreshTokenStore refreshTokenStore,
            IRefreshTokenService refreshService,
            UserManager<AppUser> userManager,
            IConfiguration cfg)
        {
            _accessTokenService = accessToken;
            _refreshTokenStore = refreshTokenStore;
            _refreshTokenService = refreshService;
            _userManager = userManager;
            _cfg = cfg;
        }

        public TokenValidationResponse ValidateAccessToken(string accessToken)
            => _accessTokenService.ValidateAccessToken(accessToken);

        public async Task<TokenPair> CreateSessionAsync(AppUser user, string? ip)
        {
            var access = await _accessTokenService.GenerateAccessToken(user);

            var accessExpiry = DateTime.UtcNow.AddMinutes(_cfg.GetValue<int>("AuthSetting:AccessTokenExpiryMinutes"));
            var refreshExpiry = DateTime.UtcNow.AddDays(_cfg.GetValue<int>("AuthSetting:RefreshTokenExpiryDays"));

            var refresh = Guid.NewGuid().ToString("N"); // compact, safe

            await _refreshTokenStore.SaveAsync(user.Id, refresh, ip, refreshExpiry);

            return new TokenPair(access, refresh, accessExpiry, refreshExpiry);
        }

        public async Task<TokenPair> RefreshSessionAsync(string accessToken, string refreshToken, string? ip)
        {
            var tokenValidation = _accessTokenService.ValidateAccessToken(accessToken);

            // If access is still valid, you may decide to refuse refresh to avoid needless rotation.
            // Here we allow refresh only when expired (typical pattern) not required but recommended.
            // if (tokenValidation.IsValid) throw new InvalidOperationException("Access token still valid.");

            var record = await _refreshTokenStore.GetAsync(refreshToken);
            if (record == null) throw new UnauthorizedAccessException("Refresh token not found.");
            if (record.ExpiresAt <= DateTime.UtcNow) throw new UnauthorizedAccessException("Refresh token expired.");

            // Optional: bind to IP if you want stricter security
            // if (!string.IsNullOrEmpty(record.Ip) && !string.IsNullOrEmpty(ip) && record.Ip != ip) throw ...

            // Rotation: revoke old refresh token and issue a new one
            await _refreshTokenStore.RevokeAsync(refreshToken);

            var user = await _userManager.FindByIdAsync(record.UserId.ToString());
            if (user == null) throw new UnauthorizedAccessException("User not found.");

            return await CreateSessionAsync(user, ip);
        }


        

        public async Task<TokenPair> GenerateTokenPairAsync(AppUser user, string? ip)
        {
            var refreshExp = int.Parse(_cfg.GetSection("AuthSetting:RefreshTokenExpiryDays").Value);
            var accessExp = int.Parse(_cfg.GetSection("AuthSetting:AccessTokenExpiryMinutes").Value);

            var access = await _accessTokenService.GenerateAccessToken(user);
            var refresh = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, ip);

            var tokenPair = new TokenPair(
            
                access,
                refresh,
                DateTime.Now.AddDays(refreshExp),
                DateTime.Now.AddMinutes(accessExp)
            );
            return tokenPair;// your original method returns only access token
        }

        public async Task<TokenPair> RefreshTokenPairAsync(string accessToken, string refreshToken)
        {
            var valid = await _refreshTokenService.ValidateRefreshTokenAsync(refreshToken);
            if (!valid) throw new UnauthorizedAccessException("Refresh token invalid");

            var refreshData = await _refreshTokenService.GetRefreshTokenAsync(refreshToken);
            if (refreshData == null) throw new UnauthorizedAccessException("Refresh token not found");

            // Revoke old token
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

            // Generate new tokens
            var user = new AppUser { Id = refreshData.UserId }; // you can load from DB if needed
            return await GenerateTokenPairAsync(user, null);
        }

        public string? GetUserIdFromAccessToken(string accessToken)
        {
            return _accessTokenService.GetUserIdFromToken(accessToken);
        }

        public DateTime GetAccessTokenExpiry(string accessToken)
        {
            return _accessTokenService.GetExpirationFromToken(accessToken);
        }
    }

}
