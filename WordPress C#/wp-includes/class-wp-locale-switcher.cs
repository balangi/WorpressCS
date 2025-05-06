using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class LocaleSwitcher
{
    /// <summary>
    /// پشته Locale
    /// </summary>
    private readonly Stack<(string Locale, int? UserId)> _stack = new();

    /// <summary>
    /// Locale اصلی
    /// </summary>
    private readonly string _originalLocale;

    /// <summary>
    /// لیست تمام زبان‌های موجود
    /// </summary>
    private readonly List<string> _availableLanguages;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<LocaleSwitcher> _logger;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public LocaleSwitcher(ILogger<LocaleSwitcher> logger)
    {
        _logger = logger;
        _originalLocale = DetermineLocale();
        _availableLanguages = new List<string> { "en_US" }; // مثال: افزودن زبان‌های موجود
        Initialize();
    }

    /// <summary>
    /// مقداردهی اولیه
    /// </summary>
    private void Initialize()
    {
        // اضافه کردن فیلترها
        AddFilter("locale", FilterLocale);
        AddFilter("determine_locale", FilterLocale);
    }

    /// <summary>
    /// تعویض Locale به Locale جدید
    /// </summary>
    public bool SwitchToLocale(string locale, int? userId = null)
    {
        var currentLocale = DetermineLocale();
        if (currentLocale == locale)
        {
            return false;
        }

        if (!_availableLanguages.Contains(locale))
        {
            return false;
        }

        _stack.Push((locale, userId));
        ChangeLocale(locale);

        OnSwitchLocale?.Invoke(locale, userId);
        return true;
    }

    /// <summary>
    /// تعویض Locale به Locale کاربر
    /// </summary>
    public bool SwitchToUserLocale(int userId)
    {
        var userLocale = GetUserLocale(userId);
        return SwitchToLocale(userLocale, userId);
    }

    /// <summary>
    /// بازگرداندن Locale قبلی
    /// </summary>
    public string RestorePreviousLocale()
    {
        if (_stack.Count == 0)
        {
            return null;
        }

        var previousEntry = _stack.Pop();
        var previousLocale = previousEntry.Locale;

        var currentEntry = _stack.Count > 0 ? _stack.Peek() : (Locale: _originalLocale, UserId: (int?)null);
        var currentLocale = currentEntry.Locale;

        ChangeLocale(currentLocale);

        OnRestorePreviousLocale?.Invoke(currentLocale, previousLocale);
        return currentLocale;
    }

    /// <summary>
    /// بازگرداندن Locale اصلی
    /// </summary>
    public string RestoreCurrentLocale()
    {
        if (_stack.Count == 0)
        {
            return null;
        }

        _stack.Clear();
        _stack.Push((_originalLocale, null));
        return RestorePreviousLocale();
    }

    /// <summary>
    /// آیا Locale تعویض شده است؟
    /// </summary>
    public bool IsSwitched()
    {
        return _stack.Count > 0;
    }

    /// <summary>
    /// دریافت Locale فعلی
    /// </summary>
    public string GetSwitchedLocale()
    {
        return _stack.Count > 0 ? _stack.Peek().Locale : null;
    }

    /// <summary>
    /// دریافت شناسه کاربر مرتبط با Locale فعلی
    /// </summary>
    public int? GetSwitchedUserId()
    {
        return _stack.Count > 0 ? _stack.Peek().UserId : null;
    }

    /// <summary>
    /// فیلتر Locale
    /// </summary>
    private string FilterLocale(string locale)
    {
        var switchedLocale = GetSwitchedLocale();
        return switchedLocale ?? locale;
    }

    /// <summary>
    /// بارگذاری ترجمه‌ها برای Locale
    /// </summary>
    private void LoadTranslations(string locale)
    {
        // مثال: بارگذاری ترجمه‌ها از Resource Files یا Database
        Console.WriteLine($"Loading translations for locale: {locale}");
    }

    /// <summary>
    /// تغییر Locale
    /// </summary>
    private void ChangeLocale(string locale)
    {
        LoadTranslations(locale);

        // مثال: تغییر Locale در سیستم
        Console.WriteLine($"Changing locale to: {locale}");

        OnChangeLocale?.Invoke(locale);
    }

    /// <summary>
    /// دریافت Locale کاربر
    /// </summary>
    private string GetUserLocale(int userId)
    {
        // مثال: دریافت Locale کاربر از Database
        return "fa_IR"; // Locale فرضی کاربر
    }

    /// <summary>
    /// دریافت Locale فعلی
    /// </summary>
    private string DetermineLocale()
    {
        // مثال: دریافت Locale فعلی از تنظیمات سیستم
        return "en_US"; // Locale فرضی فعلی
    }

    /// <summary>
    /// اضافه کردن فیلتر
    /// </summary>
    private void AddFilter(string filterName, Func<string, string> filter)
    {
        // شبیه‌سازی اضافه کردن فیلتر
        Console.WriteLine($"Adding filter: {filterName}");
    }

    /// <summary>
    /// رویداد هنگام تعویض Locale
    /// </summary>
    public event Action<string, int?> OnSwitchLocale;

    /// <summary>
    /// رویداد هنگام بازگرداندن Locale قبلی
    /// </summary>
    public event Action<string, string> OnRestorePreviousLocale;

    /// <summary>
    /// رویداد هنگام تغییر Locale
    /// </summary>
    public event Action<string> OnChangeLocale;
}