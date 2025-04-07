using FibonacciAPI.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FibonacciAPI.Services;

public class FibonacciMemoryCache : IFibonacciCache
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _expiration;

    public FibonacciMemoryCache(IMemoryCache cache, IConfiguration config)
    {
        _cache = cache;
        var minutes = config.GetValue<int>("CacheSettings:ExpirationMinutes", 10); 
        _expiration = TimeSpan.FromMinutes(minutes);
    }

    public bool TryGet(int index, out long value) => _cache.TryGetValue(index, out value);

    public void Set(int index, long value) => _cache.Set(index, value, _expiration);
}
