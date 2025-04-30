public class FeedCacheItem
{
    public int Id { get; set; }
    public string Location { get; set; } // URL مکان فید
    public string Filename { get; set; } // شناسه منحصر به فرد برای کش
    public string Extension { get; set; } // نوع فایل ('spi' یا 'spc')
    public DateTime CachedAt { get; set; } // زمان کش‌سازی
}