public interface IFeedCache
{
    bool TryGetValue<T>(string key, out T value);
    void Set<T>(string key, T value, TimeSpan expiration);
}

public class MemoryFeedCache : IFeedCache
{
    private readonly IMemoryCache _cache;

    public MemoryFeedCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        _cache.Set(key, value, expiration);
    }
}