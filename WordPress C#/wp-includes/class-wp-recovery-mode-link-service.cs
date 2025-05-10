using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RecoveryModeLinkService
{
    /// <summary>
    /// نام Action برای ورود به حالت بازیابی
    /// </summary>
    private const string LoginActionEnter = "enter_recovery_mode";

    /// <summary>
    /// نام Action برای ورود شده به حالت بازیابی
    /// </summary>
    private const string LoginActionEntered = "entered_recovery_mode";

    /// <summary>
    /// سرویس مدیریت کلیدهای حالت بازیابی
    /// </summary>
    private readonly RecoveryModeKeyService _keyService;

    /// <summary>
    /// سرویس مدیریت Cookie حالت بازیابی
    /// </summary>
    private readonly RecoveryModeCookieService _cookieService;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<RecoveryModeLinkService> _logger;

    /// <summary>
    /// HttpContext
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public RecoveryModeLinkService(
        RecoveryModeCookieService cookieService,
        RecoveryModeKeyService keyService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<RecoveryModeLinkService> logger)
    {
        _cookieService = cookieService;
        _keyService = keyService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// تولید لینک حالت بازیابی
    /// </summary>
    public string GenerateUrl()
    {
        var token = _keyService.GenerateRecoveryModeToken();
        var key = _keyService.GenerateAndStoreRecoveryModeKey(token);

        return GetRecoveryModeBeginUrl(token, key);
    }

    /// <summary>
    /// مدیریت وارد شدن به حالت بازیابی
    /// </summary>
    public void HandleBeginLink(int ttl)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext.Request.Path != "/wp-login.php")
        {
            return;
        }

        var query = httpContext.Request.Query;

        if (!query.ContainsKey("action") || !query.ContainsKey("rm_token") || !query.ContainsKey("rm_key"))
        {
            return;
        }

        if (query["action"] != LoginActionEnter)
        {
            return;
        }

        var token = query["rm_token"];
        var key = query["rm_key"];

        var validationResult = _keyService.ValidateRecoveryModeKey(token, key, ttl);

        if (!validationResult.IsSuccess)
        {
            _logger.LogError($"Failed to validate recovery mode key: {validationResult.Error}");
            httpContext.Response.StatusCode = 400;
            return;
        }

        _cookieService.SetCookie();

        var redirectUrl = AddQueryParameter("/wp-login.php", "action", LoginActionEntered);
        httpContext.Response.Redirect(redirectUrl);
        httpContext.Response.CompleteAsync();
    }

    /// <summary>
    /// ساخت URL برای شروع حالت بازیابی
    /// </summary>
    private string GetRecoveryModeBeginUrl(string token, string key)
    {
        var baseUrl = "/wp-login.php";
        var url = AddQueryParameter(baseUrl, "action", LoginActionEnter);
        url = AddQueryParameter(url, "rm_token", token);
        url = AddQueryParameter(url, "rm_key", key);

        return url;
    }

    /// <summary>
    /// اضافه کردن پارامتر به URL
    /// </summary>
    private string AddQueryParameter(string url, string key, string value)
    {
        var uriBuilder = new UriBuilder(url);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query[key] = value;
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}