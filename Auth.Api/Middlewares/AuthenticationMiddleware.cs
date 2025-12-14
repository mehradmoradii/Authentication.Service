

using Auth.Domain.AppUsers.Aggregate;
using Auth.Infrastracture.TokenServices;
using Auth.Infrastracture.TokenServices.AccessTokenServices;
using Auth.Infrastracture.TokenServices.RefreshTokenServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.JsonWebTokens;
using RTools_NTS.Util;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

public class AuthValidationMiddleware : IMiddleware
{
    private readonly ITokenService _tokenService;
    private readonly IAccessTokenService _AccessTokenService;
    private readonly IRefreshTokenService _RefreshTokenService;
    private readonly IDatabase _redis;
    private readonly IDistributedCache _cache;


    private readonly IConfiguration _config;

    public AuthValidationMiddleware(
    IRefreshTokenService refreshTokenService,
    IAccessTokenService accessTokenService,
    ITokenService tokenService,
    IConfiguration config,
    IDistributedCache distCache,
    IConnectionMultiplexer redis)
    {
        _tokenService = tokenService;
        _RefreshTokenService = refreshTokenService;
        _AccessTokenService = accessTokenService;
        _config = config;

        _cache = distCache;             // simple caching
        _redis = redis.GetDatabase();   // full Redis operations
    }



    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var bearerToken = context.Request.Headers.Authorization
            .FirstOrDefault()?.Replace("Bearer ", "");

        var refreshToken = context.Request.Cookies["refresh_token"];
        var ip = context.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(bearerToken))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Missing access token");
            return;
        }

        // 1️⃣ Validate access token
        var accessValidation = _AccessTokenService.ValidateAccessToken(bearerToken);

        if (accessValidation.IsValid)
        {
            // Attach simple user principal
            AttachUser(context, accessValidation.UserId!);
            await next(context);
            return;
        }

        // 2️⃣ If token invalid (not expired) → reject
        if (accessValidation.Error != "expired")
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid token");
            return;
        }

        // 3️⃣ Token expired → requires refresh token
        if (string.IsNullOrEmpty(refreshToken))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token expired");
            return;
        }
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(bearerToken);
        var jti = jwt.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;

        if (await _redis.KeyExistsAsync($"blacklist:{jti}"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token is blacklisted");
            return;
        }



        var redisKey = $"refresh_token:{accessValidation.UserId}:{refreshToken}";
        var redisData = await _cache.GetStringAsync(redisKey);

        if (redisData == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Refresh token invalid");
            return;
        }

        var session = JsonSerializer.Deserialize<RefreshTokenRecord>(redisData);
        if (session == null || session.Ip != ip)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Refresh token invalid");
            return;
        }

        // 4️⃣ Generate new token pair
        var userId = Guid.Parse(accessValidation.UserId!);
        var user = new AppUser { Id = userId }; // your userManager will load anyway

        var newToken = await _tokenService.GenerateTokenPairAsync(user, ip);

        // 5️⃣ Store new refresh token in redis
        var newRedisKey = $"refresh_token:{user.Id}:{newToken.RefreshToken}";
        var ttl = newToken.RefreshTokenExpiry - DateTime.Now;

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        await _cache.SetStringAsync(
            newRedisKey,
            JsonSerializer.Serialize(new
            {
                UserId = user.Id,
                RefreshToken = newToken.RefreshToken,
                Ip = ip,
                Expiration = newToken.RefreshTokenExpiry.ToString("O")
            }),
            options
        );

        // 6️⃣ Delete old refresh token
        await _cache.RemoveAsync(redisKey);

        // 7️⃣ Attach user principal
        AttachUser(context, user.Id.ToString());

        // 8️⃣ Send the new access token to client
        context.Response.Headers["X-New-Access-Token"] = newToken.AccessToken;

        await next(context);
    }


    private void AttachUser(HttpContext context, string userId)
    {
        var identity = new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "jwt");

        context.User = new ClaimsPrincipal(identity);
    }
}

