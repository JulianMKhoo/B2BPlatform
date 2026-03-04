using System.Text.Json;
using B2BPlatform.Shared.Interfaces.Services;
using StackExchange.Redis;

namespace B2BPlatform.Infrastructure.Services;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        if (expiry.HasValue)
            await _db.StringSetAsync(key, json, new Expiration(expiry.Value));
        else
            await _db.StringSetAsync(key, json);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}
