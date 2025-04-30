public class HttpCookieModel
{
    public int Id { get; set; }
    public string Name { get; set; } // نام کوکی
    public string Value { get; set; } // مقدار کوکی
    public DateTime? Expires { get; set; } // زمان انقضا
    public string Path { get; set; } // مسیر کوکی
    public string Domain { get; set; } // دامنه کوکی
    public string Port { get; set; } // پورت کوکی
    public bool HostOnly { get; set; } // فلگ host-only
}