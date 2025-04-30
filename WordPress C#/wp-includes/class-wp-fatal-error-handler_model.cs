public class FatalErrorLog
{
    public int Id { get; set; }
    public string Type { get; set; } // نوع خطا
    public string Message { get; set; } // پیام خطا
    public DateTime Timestamp { get; set; } // زمان رخ‌دادن خطا
}