using FibonacciAPI.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public class FibonacciMemoryCache : IFibonacciCache
{
    private readonly IMemoryCache _cache;

    public FibonacciMemoryCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGet(int index, out long value)
    {
        return _cache.TryGetValue(index, out value);
    }

    public void Set(int index, long value)
    {
        _cache.Set(index, value, TimeSpan.FromMinutes(10));
    }
}