using System;
using System.Text.RegularExpressions;
using Ganss.XSS; // HtmlSanitizer library

public class SanitizationService
{
    private readonly AppDbContext _context;

    public SanitizationService(AppDbContext context)
    {
        _context = context;
    }

    // پاک‌سازی داده‌ها
    public string Sanitize(string data, int type, string baseUri = "")
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return string.Empty;
        }

        data = data.Trim();

        // بررسی نوع داده
        if ((type & ConstructType.MAYBE_HTML) != 0)
        {
            if (Regex.IsMatch(data, @"(&(#(x[0-9a-fA-F]+|[0-9]+)|[a-zA-Z0-9]+)|<\/[A-Za-z][^\x09\x0A\x0B\x0C\x0D\x20\x2F\x3E]*>"))
            {
                type |= ConstructType.HTML;
            }
            else
            {
                type |= ConstructType.TEXT;
            }
        }

        // پاک‌سازی داده‌های Base64
        if ((type & ConstructType.BASE64) != 0)
        {
            try
            {
                data = Convert.ToBase64String(Convert.FromBase64String(data));
            }
            catch
            {
                data = string.Empty; // در صورت خطا، داده خالی برگردانده می‌شود
            }
        }

        // پاک‌سازی داده‌های HTML یا XHTML
        if ((type & (ConstructType.HTML | ConstructType.XHTML)) != 0)
        {
            var sanitizer = new HtmlSanitizer();
            data = sanitizer.Sanitize(data);

            // تغییر کدگذاری اگر لازم باشد
            if (!string.Equals("UTF-8", "DesiredEncoding", StringComparison.OrdinalIgnoreCase))
            {
                data = ChangeEncoding(data, "UTF-8", "DesiredEncoding");
            }

            // ذخیره داده پاک‌سازی‌شده در پایگاه داده
            SaveSanitizedData(data);
            return data;
        }

        // پاک‌سازی سایر انواع داده‌ها
        return ParentSanitize(data, type, baseUri);
    }

    // پاک‌سازی داده‌های پایه
    private string ParentSanitize(string data, int type, string baseUri)
    {
        // اینجا می‌توانید پیاده‌سازی پایه‌ای برای پاک‌سازی اضافه کنید
        return data;
    }

    // تغییر کدگذاری داده‌ها
    private string ChangeEncoding(string data, string fromEncoding, string toEncoding)
    {
        // اینجا می‌توانید تغییر کدگذاری را پیاده‌سازی کنید
        return data;
    }

    // ذخیره داده پاک‌سازی‌شده در پایگاه داده
    private void SaveSanitizedData(string sanitizedContent)
    {
        var sanitizedData = new SanitizedData
        {
            OriginalData = sanitizedContent,
            SanitizedContent = sanitizedContent,
            SanitizedTime = DateTime.UtcNow
        };
        _context.SanitizedData.Add(sanitizedData);
        _context.SaveChanges();
    }
}

// تعریف انواع داده‌ها
public static class ConstructType
{
    public const int MAYBE_HTML = 1;
    public const int BASE64 = 2;
    public const int HTML = 4;
    public const int XHTML = 8;
    public const int TEXT = 16;
}