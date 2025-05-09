using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ThestralServiceBridge.Infrastructure.Cache;

public class CacheStore(IDistributedCache distributedCache, ILogger<CacheStore> logger) : ICacheStore
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ctx) where T : class
    {
        var json = await distributedCache.GetStringAsync(key, ctx);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<T>(json, _serializerOptions);
    }

    public async Task SaveAsync<T>(string key, T value, CancellationToken ctx, TimeSpan? expiration = null)
        where T : class
    {
        var json = JsonSerializer.Serialize(value, _serializerOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        logger.LogInformation("Saving value to cache with key: {Key}", key);
        await distributedCache.SetStringAsync(key, json, options, ctx);
    }
}