using System;

public class SanitizedData
{
    public int Id { get; set; }
    public string OriginalData { get; set; } // داده اصلی
    public string SanitizedContent { get; set; } // داده پاک‌سازی‌شده
    public DateTime SanitizedTime { get; set; } // زمان پاک‌سازی
}