using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using MimeKit;

public class WpPhpMailer : IDisposable
{
    /// <summary>
    /// مدیریت زبان‌ها
    /// </summary>
    private readonly IStringLocalizer _localizer;

    /// <summary>
    /// لیست پیام‌های خطا
    /// </summary>
    private readonly Dictionary<string, string> _errorStrings;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public WpPhpMailer(IStringLocalizer localizer)
    {
        _localizer = localizer;
        _errorStrings = SetLanguage();
    }

    /// <summary>
    /// تنظیم پیام‌های خطا با استفاده از بین‌المللی‌سازی
    /// </summary>
    public Dictionary<string, string> SetLanguage()
    {
        return new Dictionary<string, string>
        {
            { "authenticate", _localizer["SMTP Error: Could not authenticate."] },
            { "buggy_php", _localizer["Your version of PHP is affected by a bug that may result in corrupted messages. To fix it, switch to sending using SMTP, disable the {0} option in your {1}, or switch to MacOS or Linux, or upgrade your PHP version.", "mail.add_x_header", "php.ini"] },
            { "connect_host", _localizer["SMTP Error: Could not connect to SMTP host."] },
            { "data_not_accepted", _localizer["SMTP Error: data not accepted."] },
            { "empty_message", _localizer["Message body empty"] },
            { "encoding", _localizer["Unknown encoding: "] },
            { "execute", _localizer["Could not execute: "] },
            { "extension_missing", _localizer["Extension missing: "] },
            { "file_access", _localizer["Could not access file: "] },
            { "file_open", _localizer["File Error: Could not open file: "] },
            { "from_failed", _localizer["The following From address failed: "] },
            { "instantiate", _localizer["Could not instantiate mail function."] },
            { "invalid_address", _localizer["Invalid address: "] },
            { "invalid_header", _localizer["Invalid header name or value"] },
            { "invalid_hostentry", _localizer["Invalid hostentry: "] },
            { "invalid_host", _localizer["Invalid host: "] },
            { "mailer_not_supported", _localizer[" mailer is not supported."] },
            { "provide_address", _localizer["You must provide at least one recipient email address."] },
            { "recipients_failed", _localizer["SMTP Error: The following recipients failed: "] },
            { "signing", _localizer["Signing Error: "] },
            { "smtp_code", _localizer["SMTP code: "] },
            { "smtp_code_ex", _localizer["Additional SMTP info: "] },
            { "smtp_connect_failed", _localizer["SMTP connect() failed."] },
            { "smtp_detail", _localizer["Detail: "] },
            { "smtp_error", _localizer["SMTP server error: "] },
            { "variable_set", _localizer["Cannot set or reset variable: "] }
        };
    }

    /// <summary>
    /// ارسال ایمیل
    /// </summary>
    public void SendEmail(string from, string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from));
            message.To.Add(new MailboxAddress(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect("smtp.example.com", 587, false);
            client.Authenticate("username", "password");
            client.Send(message);
            client.Disconnect(true);
        }
        catch (Exception ex)
        {
            throw new Exception(_errorStrings["smtp_error"] + ex.Message);
        }
    }

    /// <summary>
    /// آزاد کردن منابع
    /// </summary>
    public void Dispose()
    {
        // Cleanup resources if needed
    }
}