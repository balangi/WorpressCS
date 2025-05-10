using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RecoveryModeService
{
    /// <summary>
    /// سرویس مدیریت Cookie
    /// </summary>
    private readonly RecoveryModeCookieService _cookieService;

    /// <summary>
    /// سرویس تولید کلید بازیابی
    /// </summary>
    private readonly RecoveryModeKeyService _keyService;

    /// <summary>
    /// سرویس مدیریت لینک‌های بازیابی
    /// </summary>
    private readonly RecoveryModeLinkService _linkService;

    /// <summary>
    /// سرویس ارسال ایمیل بازیابی
    /// </summary>
    private readonly RecoveryModeEmailService _emailService;

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<RecoveryModeService> _logger;

    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// آیا حالت بازیابی فعال است؟
    /// </summary>
    private bool _isActive;

    /// <summary>
    /// شناسه جلسه بازیابی
    /// </summary>
    private string _sessionId;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public RecoveryModeService(
        RecoveryModeCookieService cookieService,
        RecoveryModeKeyService keyService,
        RecoveryModeLinkService linkService,
        RecoveryModeEmailService emailService,
        IMemoryCache cache,
        ILogger<RecoveryModeService> logger,
        AppDbContext context)
    {
        _cookieService = cookieService;
        _keyService = keyService;
        _linkService = linkService;
        _emailService = emailService;
        _cache = cache;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// مقداردهی اولیه حالت بازیابی
    /// </summary>
    public void Initialize()
    {
        if (_cookieService.IsCookieSet())
        {
            HandleCookie();
        }
        else
        {
            _linkService.HandleBeginLink(GetLinkTtl());
        }
    }

    /// <summary>
    /// بررسی فعال بودن حالت بازیابی
    /// </summary>
    public bool IsActive()
    {
        return _isActive;
    }

    /// <summary>
    /// دریافت شناسه جلسه بازیابی
    /// </summary>
    public string GetSessionId()
    {
        return _isActive ? _sessionId : string.Empty;
    }

    /// <summary>
    /// خروج از حالت بازیابی
    /// </summary>
    public bool ExitRecoveryMode()
    {
        if (!_isActive)
        {
            return false;
        }

        _emailService.ClearRateLimit();
        _cookieService.ClearCookie();
        ClearPausedExtensions();
        _isActive = false;
        return true;
    }

    /// <summary>
    /// پاک کردن افزونه‌ها و قالب‌های متوقف‌شده
    /// </summary>
    private void ClearPausedExtensions()
    {
        var pausedPlugins = _context.PausedExtensions.Where(pe => pe.Type == "plugin").ToList();
        var pausedThemes = _context.PausedExtensions.Where(pe => pe.Type == "theme").ToList();

        _context.PausedExtensions.RemoveRange(pausedPlugins);
        _context.PausedExtensions.RemoveRange(pausedThemes);
        _context.SaveChanges();
    }

    /// <summary>
    /// مدیریت Cookie
    /// </summary>
    private void HandleCookie()
    {
        var validationResult = _cookieService.ValidateCookie();
        if (!validationResult.IsSuccess)
        {
            _cookieService.ClearCookie();
            throw new InvalidOperationException("Invalid recovery mode cookie.");
        }

        _isActive = true;
        _sessionId = validationResult.SessionId;
    }

    /// <summary>
    /// دریافت مدت اعتبار لینک بازیابی
    /// </summary>
    private int GetLinkTtl()
    {
        var rateLimit = GetEmailRateLimit();
        var ttl = Math.Max(rateLimit, GetEmailRateLimit());

        return ttl;
    }

    /// <summary>
    /// دریافت محدودیت زمانی ارسال ایمیل بازیابی
    /// </summary>
    private int GetEmailRateLimit()
    {
        return 86400; // یک روز (به ثانیه)
    }

    /// <summary>
    /// پاک کردن کلیدهای منقضی
    /// </summary>
    public void CleanExpiredKeys()
    {
        _keyService.CleanExpiredKeys(GetLinkTtl());
    }
}

/// <summary>
/// مدل داده‌ای Paused Extensions
/// </summary>
public class PausedExtension
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Slug { get; set; }
    public DateTime ExpirationDate { get; set; }
}