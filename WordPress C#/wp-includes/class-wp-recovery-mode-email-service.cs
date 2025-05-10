using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RecoveryModeEmailService
{
    /// <summary>
    /// نام Option برای محدودیت زمانی ارسال ایمیل
    /// </summary>
    private const string RateLimitOption = "recovery_mode_email_last_sent";

    /// <summary>
    /// سرویس لینک‌های حالت بازیابی
    /// </summary>
    private readonly RecoveryModeLinkService _linkService;

    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<RecoveryModeEmailService> _logger;

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public RecoveryModeEmailService(
        RecoveryModeLinkService linkService,
        AppDbContext context,
        ILogger<RecoveryModeEmailService> logger,
        IMemoryCache cache)
    {
        _linkService = linkService;
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// ارسال ایمیل حالت بازیابی
    /// </summary>
    public Result SendRecoveryModeEmail(int rateLimit, ErrorDetails error, ExtensionDetails extension)
    {
        var lastSent = GetLastSentTime();
        if (lastSent.HasValue && DateTime.UtcNow < lastSent.Value.AddSeconds(rateLimit))
        {
            return Result.Failure($"A recovery email was already sent. Please wait another {rateLimit} seconds.");
        }

        if (!UpdateLastSentTime())
        {
            return Result.Failure("Could not update the email last sent time.");
        }

        try
        {
            var email = BuildEmail(error, extension);
            SendEmail(email);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send recovery mode email.");
            return Result.Failure("The email could not be sent.");
        }
    }

    /// <summary>
    /// پاک کردن محدودیت زمانی ارسال ایمیل
    /// </summary>
    public bool ClearRateLimit()
    {
        return _context.Options.Remove(new Option { Key = RateLimitOption }) > 0;
    }

    /// <summary>
    /// دریافت آخرین زمان ارسال ایمیل
    /// </summary>
    private DateTime? GetLastSentTime()
    {
        var option = _context.Options.FirstOrDefault(o => o.Key == RateLimitOption);
        return option?.Value.ToDateTime();
    }

    /// <summary>
    /// به‌روزرسانی آخرین زمان ارسال ایمیل
    /// </summary>
    private bool UpdateLastSentTime()
    {
        var option = new Option { Key = RateLimitOption, Value = DateTime.UtcNow };
        _context.Options.Update(option);
        return _context.SaveChanges() > 0;
    }

    /// <summary>
    /// ساخت ایمیل
    /// </summary>
    private Email BuildEmail(ErrorDetails error, ExtensionDetails extension)
    {
        var url = _linkService.GenerateUrl();
        var blogName = "Your Blog Name"; // Replace with actual logic
        var cause = extension != null ? GetCause(extension) : string.Empty;
        var details = error != null ? GetErrorDetails(error) : string.Empty;

        var message = $@"
            Your site is experiencing a technical issue.
            Cause: {cause}
            Details: {details}
            Recovery Link: {url}";

        return new Email
        {
            To = GetRecoveryModeEmailAddress(),
            Subject = $"[{blogName}] Your Site is Experiencing a Technical Issue",
            Body = message
        };
    }

    /// <summary>
    /// دریافت آدرس ایمیل مقصد
    /// </summary>
    private string GetRecoveryModeEmailAddress()
    {
        return "admin@example.com"; // Replace with actual logic
    }

    /// <summary>
    /// دریافت علت خطا
    /// </summary>
    private string GetCause(ExtensionDetails extension)
    {
        return $"{extension.Type} - {extension.Slug}";
    }

    /// <summary>
    /// دریافت جزئیات خطا
    /// </summary>
    private string GetErrorDetails(ErrorDetails error)
    {
        return error.Message; // Replace with actual logic
    }

    /// <summary>
    /// ارسال ایمیل
    /// </summary>
    private void SendEmail(Email email)
    {
        using var smtpClient = new SmtpClient("smtp.example.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential("username", "password"),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("noreply@example.com"),
            Subject = email.Subject,
            Body = email.Body,
            IsBodyHtml = false
        };

        mailMessage.To.Add(email.To);
        smtpClient.Send(mailMessage);
    }
}

/// <summary>
/// مدل داده‌ای Option
/// </summary>
public class Option
{
    public string Key { get; set; }
    public string Value { get; set; }
}

/// <summary>
/// مدل داده‌ای ایمیل
/// </summary>
public class Email
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}

/// <summary>
/// جزئیات خطا
/// </summary>
public class ErrorDetails
{
    public string Message { get; set; }
}

/// <summary>
/// جزئیات افزونه یا قالب
/// </summary>
public class ExtensionDetails
{
    public string Slug { get; set; }
    public string Type { get; set; }
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