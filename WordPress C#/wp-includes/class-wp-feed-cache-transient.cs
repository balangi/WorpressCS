using System;
using Microsoft.Extensions.Logging;

public class FeedCacheTransient
{
    private readonly AppDbContext _context;
    private readonly ILogger<FeedCacheTransient> _logger;

    public string Name { get; }
    public string ModName { get; }
    public int Lifetime { get; }

    public FeedCacheTransient(string location, string name, string type, AppDbContext context, ILogger<FeedCacheTransient> logger)
    {
        _context = context;
        _logger = logger;

        Name = $"feed_{name}";
        ModName = $"feed_mod_{name}";
        Lifetime = 43200; // Default: 12 hours

        // Apply filters for lifetime (if needed)
        Lifetime = ApplyLifetimeFilter(Lifetime, name);
    }

    // ذخیره داده‌ها در کش
    public bool Save(object data)
    {
        try
        {
            var cacheItem = new FeedCacheItem
            {
                Name = Name,
                ModName = ModName,
                Data = SerializeData(data),
                CreatedAt = DateTime.UtcNow,
                Lifetime = Lifetime
            };

            _context.FeedCacheItems.Add(cacheItem);
            _context.SaveChanges();

            _logger.LogInformation($"Data saved to cache with name: {Name}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save data to cache: {ex.Message}");
            return false;
        }
    }

    // بارگذاری داده‌ها از کش
    public object Load()
    {
        try
        {
            var cacheItem = _context.FeedCacheItems.FirstOrDefault(c => c.Name == Name && c.CreatedAt.AddSeconds(c.Lifetime) > DateTime.UtcNow);
            if (cacheItem == null)
            {
                _logger.LogWarning($"No valid cache found for name: {Name}");
                return null;
            }

            _logger.LogInformation($"Data loaded from cache with name: {Name}");
            return DeserializeData(cacheItem.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to load data from cache: {ex.Message}");
            return null;
        }
    }

    // دریافت زمان آخرین تغییر
    public DateTime Mtime()
    {
        try
        {
            var cacheItem = _context.FeedCacheItems.FirstOrDefault(c => c.ModName == ModName && c.CreatedAt.AddSeconds(c.Lifetime) > DateTime.UtcNow);
            if (cacheItem == null)
            {
                _logger.LogWarning($"No valid mod time found for name: {ModName}");
                return DateTime.MinValue;
            }

            _logger.LogInformation($"Mod time retrieved for name: {ModName}");
            return cacheItem.CreatedAt;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve mod time: {ex.Message}");
            return DateTime.MinValue;
        }
    }

    // به‌روزرسانی زمان آخرین تغییر
    public bool Touch()
    {
        try
        {
            var cacheItem = _context.FeedCacheItems.FirstOrDefault(c => c.ModName == ModName);
            if (cacheItem != null)
            {
                cacheItem.CreatedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
            else
            {
                cacheItem = new FeedCacheItem
                {
                    Name = ModName,
                    ModName = ModName,
                    Data = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    Lifetime = Lifetime
                };
                _context.FeedCacheItems.Add(cacheItem);
                _context.SaveChanges();
            }

            _logger.LogInformation($"Mod time updated for name: {ModName}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to update mod time: {ex.Message}");
            return false;
        }
    }

    // حذف داده‌های کش
    public bool Unlink()
    {
        try
        {
            var items = _context.FeedCacheItems.Where(c => c.Name == Name || c.ModName == ModName).ToList();
            _context.FeedCacheItems.RemoveRange(items);
            _context.SaveChanges();

            _logger.LogInformation($"Cache items deleted for name: {Name} and mod name: {ModName}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to delete cache items: {ex.Message}");
            return false;
        }
    }

    // فیلتر مدت زمان اعتبار کش
    private int ApplyLifetimeFilter(int defaultLifetime, string name)
    {
        // شبیه‌سازی فیلتر `wp_feed_cache_transient_lifetime`
        // در اینجا می‌توانید منطق سفارشی خود را اضافه کنید
        return defaultLifetime;
    }

    // سریال‌سازی داده‌ها
    private string SerializeData(object data)
    {
        return System.Text.Json.JsonSerializer.Serialize(data);
    }

    // دی‌سریال‌سازی داده‌ها
    private object DeserializeData(string data)
    {
        return System.Text.Json.JsonSerializer.Deserialize<object>(data);
    }
}