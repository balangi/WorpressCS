using System;
using System.Collections.Generic;

public class HttpRequestsHooks
{
    /// <summary>
    /// URL درخواست
    /// </summary>
    protected string Url { get; set; }

    /// <summary>
    /// داده‌های درخواست در قالب WP_Http
    /// </summary>
    protected Dictionary<string, object> Request { get; set; }

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public HttpRequestsHooks(string url, Dictionary<string, object> request)
    {
        Url = url;
        Request = request;
    }

    /// <summary>
    /// ارسال هک Requests به اکشن‌های WordPress
    /// </summary>
    public bool Dispatch(string hook, List<object> parameters = null)
    {
        if (parameters == null)
        {
            parameters = new List<object>();
        }

        // فراخوانی هک‌های پایه
        var result = BaseDispatch(hook, parameters);

        // مدیریت اکشن‌های قدیمی (Backward Compatibility)
        switch (hook)
        {
            case "curl.before_send":
                OnHttpApiCurl(parameters);
                break;
        }

        // فراخوانی اکشن‌های WordPress
        OnRequestsHook(hook, parameters);

        return result;
    }

    /// <summary>
    /// فراخوانی هک‌های پایه
    /// </summary>
    protected virtual bool BaseDispatch(string hook, List<object> parameters)
    {
        // در اینجا می‌توانید منطق پایه‌ای هک‌ها را پیاده‌سازی کنید
        return true;
    }

    /// <summary>
    /// مدیریت اکشن http_api_curl
    /// </summary>
    protected virtual void OnHttpApiCurl(List<object> parameters)
    {
        // شبیه‌سازی اکشن http_api_curl
        HttpApiCurl?.Invoke(parameters[0], Request, Url);
    }

    /// <summary>
    /// مدیریت اکشن requests-{hook}
    /// </summary>
    protected virtual void OnRequestsHook(string hook, List<object> parameters)
    {
        // شبیه‌سازی اکشن requests-{hook}
        RequestsHook?.Invoke(hook, parameters, Request, Url);
    }

    /// <summary>
    /// اکشن http_api_curl
    /// </summary>
    public event Action<object, Dictionary<string, object>, string> HttpApiCurl;

    /// <summary>
    /// اکشن requests-{hook}
    /// </summary>
    public event Action<string, List<object>, Dictionary<string, object>, string> RequestsHook;
}