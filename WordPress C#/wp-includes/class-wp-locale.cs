using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class LocaleManager
{
    /// <summary>
    /// نام‌های ترجمه‌شده روزهای هفته
    /// </summary>
    public Dictionary<string, string> Weekday { get; private set; } = new();

    /// <summary>
    /// نام‌های ترجمه‌شده ماه‌ها
    /// </summary>
    public Dictionary<string, string> Month { get; private set; } = new();

    /// <summary>
    /// تنظیمات مربوط به تقویم
    /// </summary>
    public Dictionary<string, string> CalendarSettings { get; private set; } = new();

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<LocaleManager> _logger;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public LocaleManager(ILogger<LocaleManager> logger)
    {
        _logger = logger;
        InitializeLocaleData();
    }

    /// <summary>
    /// مقداردهی اولیه اطلاعات منطقه‌ای
    /// </summary>
    private void InitializeLocaleData()
    {
        try
        {
            // مثال: مقداردهی نام‌های روزهای هفته
            Weekday = new Dictionary<string, string>
            {
                { "Sunday", "یکشنبه" },
                { "Monday", "دوشنبه" },
                { "Tuesday", "سه‌شنبه" },
                { "Wednesday", "چهارشنبه" },
                { "Thursday", "پنج‌شنبه" },
                { "Friday", "جمعه" },
                { "Saturday", "شنبه" }
            };

            // مثال: مقداردهی نام‌های ماه‌ها
            Month = new Dictionary<string, string>
            {
                { "January", "ژانویه" },
                { "February", "فوریه" },
                { "March", "مارس" },
                { "April", "آوریل" },
                { "May", "مه" },
                { "June", "ژوئن" },
                { "July", "ژوئیه" },
                { "August", "اوت" },
                { "September", "سپتامبر" },
                { "October", "اکتبر" },
                { "November", "نوامبر" },
                { "December", "دسامبر" }
            };

            // مثال: مقداردهی تنظیمات تقویم
            CalendarSettings = new Dictionary<string, string>
            {
                { "FirstDayOfWeek", "Saturday" },
                { "DateFormat", "yyyy/MM/dd" },
                { "TimeFormat", "HH:mm:ss" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to initialize locale data: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// دریافت نام ترجمه‌شده روز هفته
    /// </summary>
    public string GetWeekday(string day)
    {
        return Weekday.ContainsKey(day) ? Weekday[day] : day;
    }

    /// <summary>
    /// دریافت نام ترجمه‌شده ماه
    /// </summary>
    public string GetMonth(string month)
    {
        return Month.ContainsKey(month) ? Month[month] : month;
    }

    /// <summary>
    /// دریافت تنظیمات تقویم
    /// </summary>
    public string GetCalendarSetting(string key)
    {
        return CalendarSettings.ContainsKey(key) ? CalendarSettings[key] : null;
    }
}