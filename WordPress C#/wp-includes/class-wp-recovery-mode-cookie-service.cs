using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RecoveryModeCookieService
{
    /// <summary>
    /// نام Cookie حالت بازیابی
    /// </summary>
    private const string RecoveryModeCookieName = "RECOVERY_MODE_COOKIE";

    /// <summary>
    /// HttpContext
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<RecoveryModeCookieService> _logger;

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public RecoveryModeCookieService(IHttpContextAccessor httpContextAccessor, ILogger<RecoveryModeCookieService> logger, IMemoryCache cache)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// بررسی وجود Cookie
    /// </summary>
    public bool IsCookieSet()
    {
        return _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(RecoveryModeCookieName);
    }

    /// <summary>
    /// تنظیم Cookie
    /// </summary>
    public void SetCookie()
    {
        var value = GenerateCookie();
        var length = TimeSpan.FromDays(7); // یک هفته

        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.Add(length),
            Path = "/",
            HttpOnly = true,
            Secure = true,
            IsEssential = true
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append(RecoveryModeCookieName, value, cookieOptions);
    }

    /// <summary>
    /// پاک کردن Cookie
    /// </summary>
    public void ClearCookie()
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(RecoveryModeCookieName);
    }

    /// <summary>
    /// اعتبارسنجی Cookie
    /// </summary>
    public Result ValidateCookie(string cookieValue = null)
    {
        if (string.IsNullOrEmpty(cookieValue))
        {
            cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[RecoveryModeCookieName];
            if (string.IsNullOrEmpty(cookieValue))
            {
                return Result.Failure("No cookie present.");
            }
        }

        var parts = ParseCookie(cookieValue);
        if (parts.IsFailure)
        {
            return parts;
        }

        var (_, createdAt, random, signature) = parts.Value;

        if (!int.TryParse(createdAt, out var createdAtInt))
        {
            return Result.Failure("Invalid cookie format.");
        }

        var expiryTime = TimeSpan.FromDays(7).TotalSeconds; // یک هفته
        if (DateTime.UtcNow > DateTimeOffset.FromUnixTimeSeconds(createdAtInt).AddSeconds(expiryTime))
        {
            return Result.Failure("Cookie expired.");
        }

        var toSign = $"recovery_mode|{createdAt}|{random}";
        var hashed = RecoveryModeHash(toSign);

        if (!string.Equals(signature, hashed, StringComparison.Ordinal))
        {
            return Result.Failure("Invalid cookie.");
        }

        return Result.Success();
    }

    /// <summary>
    /// دریافت شناسه جلسه از Cookie
    /// </summary>
    public Result<string> GetSessionIdFromCookie(string cookieValue = null)
    {
        if (string.IsNullOrEmpty(cookieValue))
        {
            cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[RecoveryModeCookieName];
            if (string.IsNullOrEmpty(cookieValue))
            {
                return Result<string>.Failure("No cookie present.");
            }
        }

        var parts = ParseCookie(cookieValue);
        if (parts.IsFailure)
        {
            return Result<string>.Failure(parts.Error);
        }

        var (_, _, random) = parts.Value;
        return Result<string>.Success(ComputeSha1(random));
    }

    /// <summary>
    /// پارس کردن Cookie
    /// </summary>
    private Result<(string, string, string, string)> ParseCookie(string cookie)
    {
        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cookie));
            var parts = decoded.Split('|');

            if (parts.Length != 4)
            {
                return Result<(string, string, string, string)>.Failure("Invalid cookie format.");
            }

            return Result<(string, string, string, string)>.Success((parts[0], parts[1], parts[2], parts[3]));
        }
        catch
        {
            return Result<(string, string, string, string)>.Failure("Invalid cookie format.");
        }
    }

    /// <summary>
    /// تولید Cookie
    /// </summary>
    private string GenerateCookie()
    {
        var createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var random = GenerateRandomString(20);
        var toSign = $"recovery_mode|{createdAt}|{random}";
        var signed = RecoveryModeHash(toSign);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{toSign}|{signed}"));
    }

    /// <summary>
    /// هش کردن داده‌ها برای امضای Cookie
    /// </summary>
    private string RecoveryModeHash(string data)
    {
        var secret = GetAuthKey() + GetAuthSalt();
        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
        return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
    }

    /// <summary>
    /// دریافت Auth Key
    /// </summary>
    private string GetAuthKey()
    {
        return _cache.GetOrCreate("recovery_mode_auth_key", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30);
            return GenerateRandomString(64);
        });
    }

    /// <summary>
    /// دریافت Auth Salt
    /// </summary>
    private string GetAuthSalt()
    {
        return _cache.GetOrCreate("recovery_mode_auth_salt", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30);
            return GenerateRandomString(64);
        });
    }

    /// <summary>
    /// تولید رشته تصادفی
    /// </summary>
    private string GenerateRandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// محاسبه SHA1
    /// </summary>
    private string ComputeSha1(string input)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}

/// <summary>
/// نتیجه عملیات
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }

    public static Result Success() => new Result(true, null);
    public static Result Failure(string error) => new Result(false, error);

    private Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
}

/// <summary>
/// نتیجه عملیات با مقدار
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    public static Result<T> Success(T value) => new Result<T>(true, value, null);
    public static Result<T> Failure(string error) => new Result<T>(false, default, error);

    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
}