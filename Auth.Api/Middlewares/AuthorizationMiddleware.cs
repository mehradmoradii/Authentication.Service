using Auth.Infrastracture.TokenServices;
using Auth.Query.Queries.AppUsers;
using MediatR;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace Auth.Api.Middlewares;

public sealed class AuthorizationMiddleware : IMiddleware
{
    private readonly IConfiguration _config;
    private readonly ITokenService _tokenService;
    private readonly IDatabase _redis;
    private readonly IMediator _mediator;

    public AuthorizationMiddleware(
        IConfiguration config,
        ITokenService tokenService,
        IConnectionMultiplexer mux,
        IMediator mediator)
    {
        _config = config;
        _tokenService = tokenService;
        _redis = mux.GetDatabase();
        _mediator = mediator;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = (context.Request.Path.Value ?? string.Empty).ToLowerInvariant();

        var publicUrls = _config.GetSection("AuthSetting:PublicUrls").Get<List<string>>() ?? new();
        if (publicUrls.Contains(path))
        {
            await next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        var accessToken = authHeader?.StartsWith("Bearer ") == true
            ? authHeader.Substring("Bearer ".Length)
            : authHeader;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing access token");
            return;
        }

        var validation = _tokenService.ValidateAccessToken(accessToken);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);
        var jti = jwt.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
        var blacklist = await _redis.KeyExistsAsync($"blacklist:{accessToken}");
        if (blacklist)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token is blacklisted");
            return;
        }
        if (!validation.IsValid)
        {
            // try refresh (from cookie or header)
            var refreshToken = context.Request.Cookies["refresh_token"]
                               ?? context.Request.Headers["X-Refresh-Token"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token expired and refresh token missing");
                return;
            }

            var ip = context.Connection.RemoteIpAddress?.ToString();
            var newPair = await _tokenService.RefreshSessionAsync(accessToken, refreshToken, ip);

            // update request for this pipeline execution
            context.Request.Headers["Authorization"] = $"Bearer {newPair.AccessToken}";

            // (optional) update refresh cookie
            context.Response.Cookies.Append("refresh_token", newPair.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = newPair.RefreshTokenExpiry
            });

            validation = _tokenService.ValidateAccessToken(newPair.AccessToken);
        }

        if (string.IsNullOrWhiteSpace(validation.UserId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid token");
            return;
        }

        // IMPORTANT: cache check must treat empty list as a valid cached result
        var permissionKey = $"user:{validation.UserId}:permissions";
        var cached = await _redis.StringGetAsync(permissionKey);

        List<string> allowed;
        // <--- crucial: HasValue, not IsNullOrEmpty
        if (cached.HasValue)
        {
            try
            {
                allowed = JsonSerializer.Deserialize<List<string>>(cached!);
            }
            catch
            {
                allowed = null; // parsing failed, fallback to DB
            }
            if (allowed == null) // key missing or invalid
            {
                allowed = await GetUserPermissionsFromDbAsync(validation.UserId);
                await _redis.StringSetAsync(permissionKey, JsonSerializer.Serialize(allowed), TimeSpan.FromMinutes(10));
            }
            if (!allowed.Contains(path))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }
        }

        

        // attach principal (so downstream can use User.Identity etc.)
        context.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, validation.UserId) },
                "jwt"));

        await next(context);
    }


    /// <summary>
    /// Query database for user permissions (CQRS: Use query handler)
    /// </summary>
    private async Task<List<string>> GetUserPermissionsFromDbAsync(string userId)
    {
        var UserId = Guid.Parse(userId);
        var request = new GetAllUsersUrlQuery { UserId = UserId };
        var result = await _mediator.Send(request);

        // Get actual permissions from DB
        var permissions = result.Select(u => u.Url).ToList();

        // Use per-user key
        var permissionKey = $"user:{userId}:permissions";

        // Store in Redis
        await _redis.StringSetAsync(permissionKey, JsonSerializer.Serialize(permissions), TimeSpan.FromMinutes(10));

        return permissions;
    }


}

