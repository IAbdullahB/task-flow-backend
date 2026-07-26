using Microsoft.Extensions.Caching.Memory;
using TaskFlow.Application.ServicesInterfaces;

namespace TaskFlow.Infrastructure.Services;
internal class CacheService(IMemoryCache memoryCache) : ICacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        _memoryCache.Set(key, value, expiration);
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

}
