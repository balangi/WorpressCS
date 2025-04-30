using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class EmbedManager
{
    private readonly AppDbContext _context;

    public EmbedManager(AppDbContext context = null)
    {
        _context = context;
    }

    // تنظیمات پیش‌فرض Embed
    public Dictionary<string, object> DefaultSettings()
    {
        return new Dictionary<string, object>
        {
            { "use_cache", true },
            { "link_if_unknown", true },
            { "return_false_on_fail", false }
        };
    }

    // تبدیل URL به HTML Embed شده
    public string GetEmbedHtml(string url, Dictionary<string, object> settings = null)
    {
        settings ??= DefaultSettings();

        var useCache = settings.ContainsKey("use_cache") && (bool)settings["use_cache"];
        var linkIfUnknown = settings.ContainsKey("link_if_unknown") && (bool)settings["link_if_unknown"];

        // بررسی کش
        if (useCache && _context != null)
        {
            var cachedEmbed = _context.EmbedSettings.FirstOrDefault(e => e.Url == url);
            if (cachedEmbed != null)
            {
                return cachedEmbed.HtmlContent;
            }
        }

        // استفاده از oEmbed برای تولید HTML
        var html = TryOEmbed(url);

        if (!string.IsNullOrEmpty(html))
        {
            // ذخیره در کش
            if (useCache && _context != null)
            {
                var embedSetting = new EmbedSetting
                {
                    Url = url,
                    HtmlContent = html,
                    UseCache = true,
                    CachedAt = DateTime.UtcNow
                };
                _context.EmbedSettings.Add(embedSetting);
                _context.SaveChanges();
            }
            return html;
        }

        // اگر Embed شناخته نشود و تنظیم شده باشد، به صورت لینک بازگردانی شود
        if (linkIfUnknown)
        {
            return $"<a href=\"{url}\">{url}</a>";
        }

        // در صورت عدم موفقیت، مقدار خالی یا false بازگردانی شود
        return settings.ContainsKey("return_false_on_fail") && (bool)settings["return_false_on_fail"] ? null : url;
    }

    // تلاش برای استفاده از oEmbed
    private string TryOEmbed(string url)
    {
        // در اینجا می‌توانید از APIهای oEmbed یا سرویس‌های مشابه استفاده کنید
        // برای مثال، فرض کنید از یک API خارجی استفاده می‌کنیم
        var oEmbedResult = FetchOEmbedFromApi(url);

        if (!string.IsNullOrEmpty(oEmbedResult))
        {
            return oEmbedResult;
        }

        return null;
    }

    // شبیه‌سازی دریافت داده از API oEmbed
    private string FetchOEmbedFromApi(string url)
    {
        // اینجا می‌توانید از HttpClient برای درخواست به API استفاده کنید
        // برای مثال، فرض کنید API یک HTML ساده برای Embed برمی‌گرداند
        return $"<iframe src=\"{url}\" width=\"600\" height=\"400\"></iframe>";
    }

    // جستجوی URLهای قابل Embed در متن
    public string AutoEmbed(string content)
    {
        // جایگزینی URLهای مستقل با HTML Embed شده
        var regex = new Regex(@"(^|\s|>)https?://[^\s<>""']+(\s*)$", RegexOptions.Multiline);
        content = regex.Replace(content, match =>
        {
            var url = match.Groups[0].Value.Trim();
            return GetEmbedHtml(url);
        });

        // جایگزینی URLهای داخل پاراگراف
        regex = new Regex(@"(<p(?: [^>]*)?>\s*)(https?://[^\s<>""']+)(\s*<\/p>)", RegexOptions.IgnoreCase);
        content = regex.Replace(content, match =>
        {
            var url = match.Groups[2].Value.Trim();
            var embedHtml = GetEmbedHtml(url);
            return $"{match.Groups[1].Value}{embedHtml}{match.Groups[3].Value}";
        });

        return content;
    }
}