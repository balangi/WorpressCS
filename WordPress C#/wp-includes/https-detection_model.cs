using System;
using System.Collections.Generic;

public class HttpsDetectionError
{
    public string Code { get; set; }
    public string Message { get; set; }
}

public class SiteSettings
{
    public string HomeUrl { get; set; }
    public string SiteUrl { get; set; }
}

public class HttpsSupportResult
{
    public bool IsHttpsSupported { get; set; }
    public List<HttpsDetectionError> Errors { get; set; } = new List<HttpsDetectionError>();
}