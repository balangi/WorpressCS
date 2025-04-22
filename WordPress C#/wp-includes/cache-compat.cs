// CacheCompatibility.cs
using System;
using System.Collections.Generic;

public static class CacheCompatibility
{
    /// <summary>
    /// Adds multiple values to the cache in one call, if the cache keys don't already exist.
    /// </summary>
    public static Dictionary<string, bool> AddMultiple(Dictionary<string, object> data, string group = "", int expire = 0)
    {
        var results = new Dictionary<string, bool>();
        
        foreach (var item in data)
        {
            results[item.Key] = WpCache.Add(item.Key, item.Value, group, expire);
        }
        
        return results;
    }

    /// <summary>
    /// Sets multiple values to the cache in one call.
    /// Differs from AddMultiple in that it will always write data.
    /// </summary>
    public static Dictionary<string, bool> SetMultiple(Dictionary<string, object> data, string group = "", int expire = 0)
    {
        var results = new Dictionary<string, bool>();
        
        foreach (var item in data)
        {
            results[item.Key] = WpCache.Set(item.Key, item.Value, group, expire);
        }
        
        return results;
    }

    /// <summary>
    /// Retrieves multiple values from the cache in one call.
    /// </summary>
    public static Dictionary<string, object> GetMultiple(IEnumerable<string> keys, string group = "", bool force = false)
    {
        var results = new Dictionary<string, object>();
        
        foreach (var key in keys)
        {
            results[key] = WpCache.Get(key, group, force, out _);
        }
        
        return results;
    }

    /// <summary>
    /// Deletes multiple values from the cache in one call.
    /// </summary>
    public static Dictionary<string, bool> DeleteMultiple(IEnumerable<string> keys, string group = "")
    {
        var results = new Dictionary<string, bool>();
        
        foreach (var key in keys)
        {
            results[key] = WpCache.Delete(key, group);
        }
        
        return results;
    }

    /// <summary>
    /// Removes all cache items from the in-memory runtime cache.
    /// </summary>
    public static bool FlushRuntime()
    {
        if (!Supports("flush_runtime"))
        {
            // Log warning similar to _doing_it_wrong in WordPress
            Console.WriteLine("Warning: Your object cache implementation does not support flushing the in-memory runtime cache.");
            return false;
        }

        return WpCache.Flush();
    }

    /// <summary>
    /// Removes all cache items in a group.
    /// </summary>
    public static bool FlushGroup(string group)
    {
        if (!Supports("flush_group"))
        {
            // Log warning similar to _doing_it_wrong in WordPress
            Console.WriteLine("Warning: Your object cache implementation does not support flushing individual groups.");
            return false;
        }

        return WpCache.FlushGroup(group);
    }

    /// <summary>
    /// Determines whether the object cache implementation supports a particular feature.
    /// </summary>
    public static bool Supports(string feature)
    {
        // By default return false for all features in compatibility mode
        return false;
    }
}