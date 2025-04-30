public class HttpRequestLog
{
    public int Id { get; set; }
    public string Url { get; set; } // URL درخواست
    public string Method { get; set; } // متد HTTP (GET, POST, ...)
    public DateTime RequestedAt { get; set; } // زمان درخواست
    public int StatusCode { get; set; } // کد وضعیت HTTP
    public string Response { get; set; } // پاسخ دریافتی
}