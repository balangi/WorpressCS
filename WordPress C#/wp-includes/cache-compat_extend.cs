// WpCacheExtended.cs
using System;
using System.Collections.Generic;

public static class WpCacheExtended
{
    /// <summary>
    /// Initializes the cache with compatibility functions if needed
    /// </summary>
    public static void InitCompatibility()
    {
        // If the cache doesn't natively support multiple operations,
        // use our compatibility implementations
        if (!WpCache.Supports("add_multiple"))
        {
            WpCache.AddMultiple = CacheCompatibility.AddMultiple;
        }
        
        if (!WpCache.Supports("set_multiple"))
        {
            WpCache.SetMultiple = CacheCompatibility.SetMultiple;
        }
        
        if (!WpCache.Supports("get_multiple"))
        {
            WpCache.GetMultiple = CacheCompatibility.GetMultiple;
        }
        
        if (!WpCache.Supports("delete_multiple"))
        {
            WpCache.DeleteMultiple = CacheCompatibility.DeleteMultiple;
        }
        
        if (!WpCache.Supports("flush_runtime"))
        {
            WpCache.FlushRuntime = CacheCompatibility.FlushRuntime;
        }
        
        if (!WpCache.Supports("flush_group"))
        {
            WpCache.FlushGroup = CacheCompatibility.FlushGroup;
        }
    }
}