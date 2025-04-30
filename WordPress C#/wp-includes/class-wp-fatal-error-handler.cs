using System;
using System.Collections.Generic;
using System.Linq;

public class FatalErrorHandler
{
    private readonly AppDbContext _context;

    public FatalErrorHandler(AppDbContext context = null)
    {
        _context = context;
    }

    // مدیریت خطاهای جدی
    public void Handle()
    {
        try
        {
            // شناسایی خطا
            var error = DetectError();
            if (error == null)
            {
                return;
            }

            // بررسی اینکه آیا خطا باید مدیریت شود
            if (!ShouldHandleError(error))
            {
                return;
            }

            // ثبت خطا در دیتابیس (اگر نیاز به ذخیره‌سازی باشد)
            LogError(error);

            // نمایش قالب خطا
            DisplayErrorTemplate(error);
        }
        catch (Exception ex)
        {
            // مدیریت استثناها
            Console.WriteLine($"An exception occurred: {ex.Message}");
        }
    }

    // شناسایی خطا
    private Dictionary<string, object> DetectError()
    {
        // شبیه‌سازی `error_get_last()`
        var lastError = new Dictionary<string, object>
        {
            { "type", 1 }, // E_ERROR
            { "message", "A fatal error occurred." },
            { "file", "example.php" },
            { "line", 42 }
        };

        return lastError.Count > 0 ? lastError : null;
    }

    // بررسی اینکه آیا خطا باید مدیریت شود
    private bool ShouldHandleError(Dictionary<string, object> error)
    {
        var errorTypesToHandle = new List<int> { 1, 4, 256, 4096, 4096 }; // E_ERROR, E_PARSE, etc.
        if (error.ContainsKey("type") && errorTypesToHandle.Contains((int)error["type"]))
        {
            return true;
        }

        return false;
    }

    // ثبت خطا در دیتابیس
    private void LogError(Dictionary<string, object> error)
    {
        if (_context != null)
        {
            var log = new FatalErrorLog
            {
                Type = error["type"].ToString(),
                Message = error["message"].ToString(),
                Timestamp = DateTime.UtcNow
            };
            _context.FatalErrorLogs.Add(log);
            _context.SaveChanges();
        }
    }

    // نمایش قالب خطا
    private void DisplayErrorTemplate(Dictionary<string, object> error)
    {
        Console.WriteLine("A critical error has occurred:");
        Console.WriteLine($"Type: {error["type"]}");
        Console.WriteLine($"Message: {error["message"]}");
        Console.WriteLine($"File: {error["file"]}");
        Console.WriteLine($"Line: {error["line"]}");

        // نمایش پیام‌های اختصاصی
        Console.WriteLine("Please check the admin panel for more details.");
    }
}