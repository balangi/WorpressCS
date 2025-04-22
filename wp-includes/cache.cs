// ObjectCache.cs - پیاده‌سازی پایه کش
public class ObjectCache : ICacheProvider
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
    private readonly HashSet<string> _globalGroups = new();
    private int _blogId = 1;

    protected class CacheItem
    {
        public object Data { get; set; }
        public string Group { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsExpired => Expiration > DateTime.MinValue && Expiration < DateTime.Now;
    }

    public bool Add(string key, object data, string group = "", int expire = 0)
    {
        var cacheKey = GetCacheKey(key, group);
        if (_cache.ContainsKey(cacheKey)) return false;

        return Set(key, data, group, expire);
    }

    public bool[] AddMultiple(Dictionary<string, object> data, string group = "", int expire = 0)
    {
        return data.Select(item => Add(item.Key, item.Value, group, expire)).ToArray();
    }

    public bool Replace(string key, object data, string group = "", int expire = 0)
    {
        var cacheKey = GetCacheKey(key, group);
        if (!_cache.ContainsKey(cacheKey)) return false;

        return Set(key, data, group, expire);
    }

    public bool Set(string key, object data, string group = "", int expire = 0)
    {
        var cacheKey = GetCacheKey(key, group);
        var expiration = expire > 0 ? DateTime.Now.AddSeconds(expire) : DateTime.MinValue;

        _cache[cacheKey] = new CacheItem
        {
            Data = data,
            Group = group,
            Expiration = expiration
        };

        return true;
    }

    public bool[] SetMultiple(Dictionary<string, object> data, string group = "", int expire = 0)
    {
        return data.Select(item => Set(item.Key, item.Value, group, expire)).ToArray();
    }

    public object Get(string key, string group = "", bool force = false, out bool found)
    {
        var cacheKey = GetCacheKey(key, group);
        found = _cache.TryGetValue(cacheKey, out var item);

        if (!found || item == null || item.IsExpired)
        {
            found = false;
            return null;
        }

        found = true;
        return item.Data;
    }

    public Dictionary<string, object> GetMultiple(IEnumerable<string> keys, string group = "", bool force = false)
    {
        return keys.ToDictionary(
            key => key,
            key => Get(key, group, force, out _)
        );
    }

    public bool Delete(string key, string group = "")
    {
        var cacheKey = GetCacheKey(key, group);
        return _cache.TryRemove(cacheKey, out _);
    }

    public bool[] DeleteMultiple(IEnumerable<string> keys, string group = "")
    {
        return keys.Select(key => Delete(key, group)).ToArray();
    }

    public long Increment(string key, long offset = 1, string group = "")
    {
        var cacheKey = GetCacheKey(key, group);
        var newValue = _cache.AddOrUpdate(
            cacheKey,
            _ => new CacheItem { Data = offset, Group = group },
            (_, existing) =>
            {
                var current = Convert.ToInt64(existing.Data);
                existing.Data = current + offset;
                return existing;
            });

        return Convert.ToInt64(newValue.Data);
    }

    public long Decrement(string key, long offset = 1, string group = "")
    {
        return Increment(key, -offset, group);
    }

    public bool Flush()
    {
        _cache.Clear();
        return true;
    }

    public bool FlushGroup(string group)
    {
        var itemsToRemove = _cache.Where(x => x.Value.Group == group).Select(x => x.Key).ToList();
        foreach (var key in itemsToRemove)
        {
            _cache.TryRemove(key, out _);
        }
        return true;
    }

    public void AddGlobalGroups(IEnumerable<string> groups)
    {
        foreach (var group in groups)
        {
            _globalGroups.Add(group);
        }
    }

    public void SwitchToBlog(int blogId)
    {
        _blogId = blogId;
    }

    public bool Supports(string feature)
    {
        return feature switch
        {
            "add_multiple" => true,
            "set_multiple" => true,
            "get_multiple" => true,
            "delete_multiple" => true,
            "flush_runtime" => true,
            "flush_group" => true,
            _ => false
        };
    }

    private string GetCacheKey(string key, string group)
    {
        if (_globalGroups.Contains(group))
        {
            return $"{group}:{key}";
        }
        return $"{_blogId}:{group}:{key}";
    }
}