using System;
using System.Collections.Generic;
using System.Linq;

public class HttpRequestsResponse
{
    /// <summary>
    /// پاسخ HTTP
    /// </summary>
    protected HttpResponse Response { get; set; }

    /// <summary>
    /// نام فایلی که پاسخ در آن ذخیره شده است
    /// </summary>
    protected string Filename { get; set; }

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public HttpRequestsResponse(HttpResponse response, string filename = null)
    {
        Response = response;
        Filename = filename;
    }

    /// <summary>
    /// دریافت شیء پاسخ HTTP
    /// </summary>
    public HttpResponse GetResponseObject()
    {
        return Response;
    }

    /// <summary>
    /// دریافت هدرها
    /// </summary>
    public Dictionary<string, string> GetHeaders()
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var header in Response.Headers)
        {
            headers[header.Key] = header.Value.FirstOrDefault();
        }

        return headers;
    }

    /// <summary>
    /// تنظیم تمام هدرها
    /// </summary>
    public void SetHeaders(Dictionary<string, string> headers)
    {
        Response.Headers = headers;
    }

    /// <summary>
    /// تنظیم یک هدر
    /// </summary>
    public void SetHeader(string key, string value, bool replace = true)
    {
        if (replace)
        {
            Response.Headers.Remove(key);
        }

        Response.Headers[key] = value;
    }

    /// <summary>
    /// دریافت کد وضعیت HTTP
    /// </summary>
    public int GetStatus()
    {
        return Response.StatusCode;
    }

    /// <summary>
    /// تنظیم کد وضعیت HTTP
    /// </summary>
    public void SetStatus(int code)
    {
        Response.StatusCode = Math.Abs(code);
    }

    /// <summary>
    /// دریافت داده‌های پاسخ
    /// </summary>
    public string GetData()
    {
        return Response.Body;
    }

    /// <summary>
    /// تنظیم داده‌های پاسخ
    /// </summary>
    public void SetData(string data)
    {
        Response.Body = data;
    }

    /// <summary>
    /// دریافت کوکی‌ها
    /// </summary>
    public List<HttpCookie> GetCookies()
    {
        var cookies = new List<HttpCookie>();

        foreach (var cookie in Response.Cookies)
        {
            cookies.Add(new HttpCookie
            {
                Name = cookie.Name,
                Value = Uri.UnescapeDataString(cookie.Value),
                Expires = cookie.Attributes.ContainsKey("expires") ? cookie.Attributes["expires"] : (DateTime?)null,
                Path = cookie.Attributes.ContainsKey("path") ? cookie.Attributes["path"] : null,
                Domain = cookie.Attributes.ContainsKey("domain") ? cookie.Attributes["domain"] : null,
                HostOnly = cookie.Flags.ContainsKey("host-only") ? cookie.Flags["host-only"] : false
            });
        }

        return cookies;
    }

    /// <summary>
    /// تبدیل پاسخ به آرایه
    /// </summary>
    public Dictionary<string, object> ToArray()
    {
        return new Dictionary<string, object>
        {
            { "headers", GetHeaders() },
            { "body", GetData() },
            { "response", new Dictionary<string, object>
                {
                    { "code", GetStatus() },
                    { "message", GetStatusMessage(GetStatus()) }
                }
            },
            { "cookies", GetCookies() },
            { "filename", Filename }
        };
    }

    /// <summary>
    /// دریافت پیام متناظر با کد وضعیت HTTP
    /// </summary>
    private string GetStatusMessage(int statusCode)
    {
        // لیست پیام‌های متناظر با کدهای وضعیت HTTP
        var statusMessages = new Dictionary<int, string>
        {
            { 200, "OK" },
            { 404, "Not Found" },
            { 500, "Internal Server Error" }
            // می‌توانید کدهای دیگر را نیز اضافه کنید
        };

        return statusMessages.ContainsKey(statusCode) ? statusMessages[statusCode] : "Unknown Status";
    }
}
