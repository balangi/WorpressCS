using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class PausedExtensionsStorage
{
    /// <summary>
    /// نوع افزونه یا قالب (Plugin یا Theme)
    /// </summary>
    private readonly string _type;

    /// <summary>
    /// Database Context
    /// </summary>
    private readonly RecoveryDbContext _context;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<PausedExtensionsStorage> _logger;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public PausedExtensionsStorage(string extensionType, RecoveryDbContext context, ILogger<PausedExtensionsStorage> logger)
    {
        _type = extensionType?.ToLower() == "plugin" || extensionType?.ToLower() == "theme"
            ? extensionType.ToLower()
            : throw new ArgumentException("Invalid extension type. Must be 'plugin' or 'theme'.");

        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// ثبت خطا برای یک افزونه یا قالب
    /// </summary>
    public bool Set(string extension, ErrorInfo error)
    {
        if (!IsApiLoaded())
        {
            return false;
        }

        var pausedExtensions = GetPausedExtensions();

        // اگر خطا قبلاً ذخیره شده باشد، آن را به‌روزرسانی کنید
        if (pausedExtensions.ContainsKey(extension) && pausedExtensions[extension].Equals(error))
        {
            return true;
        }

        pausedExtensions[extension] = error;

        SavePausedExtensions(pausedExtensions);
        return true;
    }

    /// <summary>
    /// حذف خطا برای یک افزونه یا قالب
    /// </summary>
    public bool Delete(string extension)
    {
        if (!IsApiLoaded())
        {
            return false;
        }

        var pausedExtensions = GetPausedExtensions();

        if (!pausedExtensions.ContainsKey(extension))
        {
            return true;
        }

        pausedExtensions.Remove(extension);

        SavePausedExtensions(pausedExtensions);
        return true;
    }

    /// <summary>
    /// دریافت خطا برای یک افزونه یا قالب
    /// </summary>
    public ErrorInfo Get(string extension)
    {
        if (!IsApiLoaded())
        {
            return null;
        }

        var pausedExtensions = GetPausedExtensions();
        return pausedExtensions.ContainsKey(extension) ? pausedExtensions[extension] : null;
    }

    /// <summary>
    /// دریافت تمام خطاها
    /// </summary>
    public Dictionary<string, ErrorInfo> GetAll()
    {
        if (!IsApiLoaded())
        {
            return new Dictionary<string, ErrorInfo>();
        }

        return GetPausedExtensions();
    }

    /// <summary>
    /// حذف تمام خطاها
    /// </summary>
    public bool DeleteAll()
    {
        if (!IsApiLoaded())
        {
            return false;
        }

        var pausedExtensions = GetPausedExtensions();
        pausedExtensions.Clear();

        SavePausedExtensions(pausedExtensions);
        return true;
    }

    /// <summary>
    /// بررسی بارگذاری API
    /// </summary>
    private bool IsApiLoaded()
    {
        // شبیه‌سازی `function_exists('get_option')`
        return true;
    }

    /// <summary>
    /// دریافت نام Option برای ذخیره خطاها
    /// </summary>
    private string GetOptionName()
    {
        // شبیه‌سازی Recovery Mode
        var sessionId = Guid.NewGuid().ToString(); // جایگزین Recovery Mode Session ID
        return $"{sessionId}_paused_extensions";
    }

    /// <summary>
    /// دریافت خطاها از دیتابیس
    /// </summary>
    private Dictionary<string, ErrorInfo> GetPausedExtensions()
    {
        var optionName = GetOptionName();
        var pausedExtensions = _context.PausedExtensions
            .Where(pe => pe.OptionName == optionName && pe.Type == _type)
            .ToDictionary(pe => pe.Extension, pe => new ErrorInfo
            {
                Type = pe.ErrorType,
                File = pe.File,
                Line = pe.Line,
                Message = pe.Message
            });

        return pausedExtensions;
    }

    /// <summary>
    /// ذخیره خطاها در دیتابیس
    /// </summary>
    private void SavePausedExtensions(Dictionary<string, ErrorInfo> pausedExtensions)
    {
        var optionName = GetOptionName();

        // حذف خطاها قبلی
        var existingRecords = _context.PausedExtensions
            .Where(pe => pe.OptionName == optionName && pe.Type == _type)
            .ToList();
        _context.PausedExtensions.RemoveRange(existingRecords);

        // اضافه کردن خطاها جدید
        foreach (var entry in pausedExtensions)
        {
            _context.PausedExtensions.Add(new PausedExtensionEntity
            {
                OptionName = optionName,
                Type = _type,
                Extension = entry.Key,
                ErrorType = entry.Value.Type,
                File = entry.Value.File,
                Line = entry.Value.Line,
                Message = entry.Value.Message
            });
        }

        _context.SaveChanges();
    }
}

/// <summary>
/// مدل داده‌ای برای خطاها
/// </summary>
public class ErrorInfo
{
    public int Type { get; set; }
    public string File { get; set; }
    public int Line { get; set; }
    public string Message { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is ErrorInfo other)
        {
            return Type == other.Type &&
                   File == other.File &&
                   Line == other.Line &&
                   Message == other.Message;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, File, Line, Message);
    }
}
