// <summary>
/// تنظیمات درخواست HTTP
/// </summary>
public class HttpRequestOptions
{
    public bool SslVerify { get; set; } = true;
    public bool StreamToFile { get; set; } = false;
    public string Filename { get; set; }
    public int LimitResponseSize { get; set; } = int.MaxValue;
    public HttpProxy Proxy { get; set; }
}

/// <summary>
/// تنظیمات پروکسی
/// </summary>
public class HttpProxy
{
    public string Host { get; set; }
    public int Port { get; set; }

    public bool IsEnabled()
    {
        return !string.IsNullOrEmpty(Host) && Port > 0;
    }
}