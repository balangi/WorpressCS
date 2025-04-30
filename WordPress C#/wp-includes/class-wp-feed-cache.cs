using System;
using Microsoft.Extensions.Logging;

public class FeedCache
{
    private readonly ILogger<FeedCache> _logger;
    private readonly AppDbContext _context;

    public FeedCache(ILogger<FeedCache> logger, AppDbContext context = null)
    {
        _logger = logger;
        _context = context;

        // ثبت اطلاعات Deprecated
        _logger.LogWarning("This class is deprecated and only loaded for backward compatibility with older versions.");
    }

    // ایجاد شیء کش جدید
    public FeedCacheTransient Create(string location, string filename, string extension)
    {
        if (_context != null)
        {
            // ذخیره اطلاعات کش در دیتابیس
            var cacheItem = new FeedCacheItem
            {
                Location = location,
                Filename = filename,
                Extension = extension,
                CachedAt = DateTime.UtcNow
            };
            _context.FeedCacheItems.Add(cacheItem);
            _context.SaveChanges();
        }

        return new FeedCacheTransient(location, filename, extension);
    }
}