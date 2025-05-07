using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class ObjectCache
{
    /// <summary>
    /// ذخیره Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// تعداد Hit‌های Cache
    /// </summary>
    public int CacheHits { get; private set; }

    /// <summary>
    /// تعداد Miss‌های Cache
    /// </summary>
    public int CacheMisses { get; private set; }

    /// <summary>
    /// لیست گروه‌های Global Cache
    /// </summary>
    private readonly HashSet<string> _globalGroups = new();

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<ObjectCache> _logger;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public ObjectCache(IMemoryCache cache, ILogger<ObjectCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// اضافه کردن گروه‌های Global Cache
    /// </summary>
    public void AddGlobalGroups(IEnumerable<string> groups)
    {
        foreach (var group in groups)
        {
            _globalGroups.Add(group);
        }
    }

    /// <summary>
    /// ذخیره مقدار در Cache
    /// </summary>
    public bool Set(string key, object value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }

            _cache.Set(key, value, options);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// دریافت مقدار از Cache
    /// </summary>
    public T Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out T value))
        {
            CacheHits++;
            return value;
        }

        CacheMisses++;
        return default;
    }

    /// <summary>
    /// اضافه کردن مقدار به Cache (اگر وجود نداشته باشد)
    /// </summary>
    public bool Add(string key, object value, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out _))
        {
            return false;
        }

        return Set(key, value, expiration);
    }

    /// <summary>
    /// حذف مقدار از Cache
    /// </summary>
    public void Delete(string key)
    {
        _cache.Remove(key);
    }

    /// <summary>
    /// حذف تمام مقادیر Cache
    /// </summary>
    public void Flush()
    {
        foreach (var key in _cache.GetKeys())
        {
            _cache.Remove(key);
        }
    }
}