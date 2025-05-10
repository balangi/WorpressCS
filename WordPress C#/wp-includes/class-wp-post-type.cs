using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class PostType
{
    /// <summary>
    /// کلید نوع پست
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// نام نمایشی نوع پست (معمولاً جمع)
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// تنظیمات Rewrite Rules
    /// </summary>
    public RewriteSettings Rewrite { get; set; } = new();

    /// <summary>
    /// آیا این نوع پست داخلی (Built-in) است؟
    /// </summary>
    public bool IsBuiltin { get; set; } = false;

    /// <summary>
    /// لینک ویرایش برای این نوع پست
    /// </summary>
    public string EditLink { get; set; } = "post.php?post=%d";

    /// <summary>
    /// قابلیت‌های مرتبط با این نوع پست
    /// </summary>
    public Capabilities Capabilities { get; set; } = new();

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<PostType> _logger;

    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public PostType(IMemoryCache cache, ILogger<PostType> logger, AppDbContext context)
    {
        _cache = cache;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// ثبت نوع پست
    /// </summary>
    public void RegisterPostType(Dictionary<string, object> args)
    {
        var postTypeName = Name;

        // اعمال فیلترها
        args = ApplyFilters("register_post_type_args", args, postTypeName);

        // ذخیره تنظیمات در دیتابیس
        var postTypeEntity = new PostTypeEntity
        {
            Name = Name,
            Label = Label,
            RewriteSlug = Rewrite.Slug,
            RewriteWithFront = Rewrite.WithFront,
            RewriteFeeds = Rewrite.Feeds,
            RewritePages = Rewrite.Pages,
            IsBuiltin = IsBuiltin,
            EditLink = EditLink
        };

        _context.PostTypes.Add(postTypeEntity);
        _context.SaveChanges();

        // اضافه کردن Rewrite Rules
        AddRewriteRules();
    }

    /// <summary>
    /// اضافه کردن Rewrite Rules
    /// </summary>
    private void AddRewriteRules()
    {
        if (Rewrite.Enabled)
        {
            var archiveSlug = Rewrite.Slug ?? Name;
            _logger.LogInformation("Adding rewrite rule for post type: {Name}", Name);

            // اضافه کردن Rewrite Rule برای صفحه اصلی
            _context.RewriteRules.Add(new RewriteRule
            {
                Regex = $"{archiveSlug}/?$",
                Query = $"index.php?post_type={Name}"
            });

            // اضافه کردن Rewrite Rule برای Feed
            if (Rewrite.Feeds)
            {
                _context.RewriteRules.Add(new RewriteRule
                {
                    Regex = $"{archiveSlug}/feed/([^/]+)/?$",
                    Query = $"index.php?post_type={Name}&feed=$1"
                });
            }

            // اضافه کردن Rewrite Rule برای صفحه‌بندی
            if (Rewrite.Pages)
            {
                _context.RewriteRules.Add(new RewriteRule
                {
                    Regex = $"{archiveSlug}/page/([0-9]+)/?$",
                    Query = $"index.php?post_type={Name}&paged=$1"
                });
            }

            _context.SaveChanges();
        }
    }

    /// <summary>
    /// حذف نوع پست
    /// </summary>
    public void UnregisterPostType()
    {
        var postTypeEntity = _context.PostTypes.FirstOrDefault(pt => pt.Name == Name);
        if (postTypeEntity != null)
        {
            _context.PostTypes.Remove(postTypeEntity);
            _context.SaveChanges();
        }

        // حذف Rewrite Rules
        RemoveRewriteRules();
    }

    /// <summary>
    /// حذف Rewrite Rules
    /// </summary>
    private void RemoveRewriteRules()
    {
        var rulesToRemove = _context.RewriteRules.Where(rr => rr.Query.Contains($"post_type={Name}")).ToList();
        _context.RewriteRules.RemoveRange(rulesToRemove);
        _context.SaveChanges();
    }

    /// <summary>
    /// اعمال فیلترها
    /// </summary>
    private Dictionary<string, object> ApplyFilters(string filterName, Dictionary<string, object> args, string postTypeName)
    {
        // شبیه‌سازی فیلترها
        return args;
    }
}

/// <summary>
/// تنظیمات Rewrite
/// </summary>
public class RewriteSettings
{
    public bool Enabled { get; set; } = true;
    public string Slug { get; set; }
    public bool WithFront { get; set; } = true;
    public bool Feeds { get; set; } = true;
    public bool Pages { get; set; } = true;
}

/// <summary>
/// قابلیت‌های مرتبط با نوع پست
/// </summary>
public class Capabilities
{
    public string Edit { get; set; }
    public string Delete { get; set; }
    public string Publish { get; set; }
}