using Auth.Infrastracture.TokenServices;
using Auth.Infrastracture.TokenServices.RefreshTokenServices;
using StackExchange.Redis;
using System.Text.Json;

public sealed class RedisRefreshTokenStore : IRefreshTokenStore
{
    private readonly IDatabase _db;

    public RedisRefreshTokenStore(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    private static string TokenKey(string token) => $"token:{token}";
    private static string UserSetKey(Guid userId) => $"user:{userId}:tokens";

    public async Task SaveAsync(Guid userId, string refreshToken, string? ip, DateTime expiresAt)
    {
        var record = new RefreshTokenRecord(userId, ip, expiresAt);
        var json = JsonSerializer.Serialize(record);

        var ttl = expiresAt - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero) ttl = TimeSpan.FromSeconds(1);

        await _db.StringSetAsync(TokenKey(refreshToken), json, ttl);
        await _db.SetAddAsync(UserSetKey(userId), refreshToken);
    }

    public async Task<RefreshTokenRecord?> GetAsync(string refreshToken)
    {
        var json = await _db.StringGetAsync(TokenKey(refreshToken));
        return json.IsNullOrEmpty ? null : JsonSerializer.Deserialize<RefreshTokenRecord>(json!);
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var rec = await GetAsync(refreshToken);
        if (rec == null) return;

        await _db.KeyDeleteAsync(TokenKey(refreshToken));
        await _db.SetRemoveAsync(UserSetKey(rec.UserId), refreshToken);
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        var members = await _db.SetMembersAsync(UserSetKey(userId));
        foreach (var m in members)
        {
            await _db.KeyDeleteAsync(TokenKey(m!));
        }
        await _db.KeyDeleteAsync(UserSetKey(userId));
    }
}
