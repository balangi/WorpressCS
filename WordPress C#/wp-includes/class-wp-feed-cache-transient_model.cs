public class FeedCacheItem
{
    public int Id { get; set; }
    public string Name { get; set; } // نام کش
    public string ModName { get; set; } // نام کش تغییرات
    public string Data { get; set; } // داده‌های کش
    public DateTime CreatedAt { get; set; } // زمان ایجاد کش
    public int Lifetime { get; set; } // مدت زمان اعتبار کش (به ثانیه)
}