using System;
using System.Collections.Generic;
using System.Text.Json;

public class HttpResponseManager
{
    /// <summary>
    /// داده‌های پاسخ
    /// </summary>
    public object Data { get; private set; }

    /// <summary>
    /// هدرهای پاسخ
    /// </summary>
    public Dictionary<string, string> Headers { get; private set; } = new();

    /// <summary>
    /// وضعیت HTTP
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public HttpResponseManager(object data = null, int status = 200, Dictionary<string, string> headers = null)
    {
        SetData(data);
        SetStatus(status);
        SetHeaders(headers ?? new Dictionary<string, string>());
    }

    /// <summary>
    /// دریافت هدرها
    /// </summary>
    public Dictionary<string, string> GetHeaders()
    {
        return Headers;
    }

    /// <summary>
    /// تنظیم تمام هدرها
    /// </summary>
    public void SetHeaders(Dictionary<string, string> headers)
    {
        Headers = headers;
    }

    /// <summary>
    /// تنظیم یک هدر
    /// </summary>
    public void SetHeader(string key, string value, bool replace = true)
    {
        if (replace || !Headers.ContainsKey(key))
        {
            Headers[key] = value;
        }
        else
        {
            Headers[key] += ", " + value;
        }
    }

    /// <summary>
    /// دریافت کد وضعیت HTTP
    /// </summary>
    public int GetStatus()
    {
        return Status;
    }

    /// <summary>
    /// تنظیم کد وضعیت HTTP
    /// </summary>
    public void SetStatus(int code)
    {
        Status = Math.Abs(code);
    }

    /// <summary>
    /// دریافت داده‌های پاسخ
    /// </summary>
    public object GetData()
    {
        return Data;
    }

    /// <summary>
    /// تنظیم داده‌های پاسخ
    /// </summary>
    public void SetData(object data)
    {
        Data = data;
    }

    /// <summary>
    /// سریال‌سازی داده‌ها به JSON
    /// </summary>
    public string JsonSerialize()
    {
        return JsonSerializer.Serialize(Data);
    }
}