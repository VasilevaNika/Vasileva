using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace WebApplication1.Services;

public class IdempotencyService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<IdempotencyService> _logger;

    public IdempotencyService(IDistributedCache cache, ILogger<IdempotencyService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<string?> GetCachedResponseAsync(string idempotencyKey)
    {
        var cacheKey = $"idempotency:{idempotencyKey}";
        return await _cache.GetStringAsync(cacheKey);
    }

    public async Task CacheResponseAsync(string idempotencyKey, string response, TimeSpan expiration)
    {
        var cacheKey = $"idempotency:{idempotencyKey}";
        await _cache.SetStringAsync(cacheKey, response, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });
    }

    public static string GenerateIdempotencyKey(HttpRequest request, string body)
    {
        var key = $"{request.Method}:{request.Path}:{body}";
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        return Convert.ToBase64String(hash);
    }
}

