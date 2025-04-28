public class Dependency
{
    public string Handle { get; set; } // شناسه منحصر به فرد وابستگی
    public string Src { get; set; } // مسیر فایل (URL یا مسیر محلی)
    public List<string> Dependencies { get; set; } // لیست وابستگی‌های این وابستگی
    public string Version { get; set; } // نسخه وابستگی
    public bool Enqueued { get; set; } // آیا وابستگی در صف قرار دارد؟
}