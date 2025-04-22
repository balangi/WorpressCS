// WpCache.cs - wrapper برای شبیه‌سازی توابع wp_cache_*
public static class WpCache
{
    private static ICacheProvider _cache = new ObjectCache();

    public static void Init(ICacheProvider cacheProvider = null)
    {
        _cache = cacheProvider ?? new ObjectCache();
    }

    public static bool Add(string key, object data, string group = "", int expire = 0)
        => _cache.Add(key, data, group, expire);

    public static bool[] AddMultiple(Dictionary<string, object> data, string group = "", int expire = 0)
        => _cache.AddMultiple(data, group, expire);

    public static bool Replace(string key, object data, string group = "", int expire = 0)
        => _cache.Replace(key, data, group, expire);

    public static bool Set(string key, object data, string group = "", int expire = 0)
        => _cache.Set(key, data, group, expire);

    public static bool[] SetMultiple(Dictionary<string, object> data, string group = "", int expire = 0)
        => _cache.SetMultiple(data, group, expire);

    public static object Get(string key, string group = "", bool force = false, out bool found)
        => _cache.Get(key, group, force, out found);

    public static Dictionary<string, object> GetMultiple(IEnumerable<string> keys, string group = "", bool force = false)
        => _cache.GetMultiple(keys, group, force);

    public static bool Delete(string key, string group = "")
        => _cache.Delete(key, group);

    public static bool[] DeleteMultiple(IEnumerable<string> keys, string group = "")
        => _cache.DeleteMultiple(keys, group);

    public static long Increment(string key, long offset = 1, string group = "")
        => _cache.Increment(key, offset, group);

    public static long Decrement(string key, long offset = 1, string group = "")
        => _cache.Decrement(key, offset, group);

    public static bool Flush()
        => _cache.Flush();

    public static bool FlushRuntime()
        => _cache.Flush();

    public static bool FlushGroup(string group)
        => _cache.FlushGroup(group);

    public static bool Supports(string feature)
        => _cache.Supports(feature);

    public static void AddGlobalGroups(IEnumerable<string> groups)
        => _cache.AddGlobalGroups(groups);

    public static void SwitchToBlog(int blogId)
        => _cache.SwitchToBlog(blogId);

    [Obsolete("Use SwitchToBlog instead")]
    public static void Reset()
    {
        // For backward compatibility
        _cache.Flush();
    }
}