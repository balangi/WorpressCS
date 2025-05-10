using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class Post
{
    /// <summary>
    /// شناسه پست
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// شناسه نویسنده پست
    /// </summary>
    public string PostAuthor { get; set; } = "0";

    /// <summary>
    /// تاریخ انتشار پست
    /// </summary>
    public DateTime PostDate { get; set; } = DateTime.MinValue;

    /// <summary>
    /// تاریخ انتشار پست (به زمان GMT)
    /// </summary>
    public DateTime PostDateGmt { get; set; } = DateTime.MinValue;

    /// <summary>
    /// محتوای پست
    /// </summary>
    public string PostContent { get; set; } = "";

    /// <summary>
    /// عنوان پست
    /// </summary>
    public string PostTitle { get; set; } = "";

    /// <summary>
    /// خلاصه پست
    /// </summary>
    public string PostExcerpt { get; set; } = "";

    /// <summary>
    /// وضعیت پست
    /// </summary>
    public string PostStatus { get; set; } = "publish";

    /// <summary>
    /// وضعیت نظرات
    /// </summary>
    public string CommentStatus { get; set; } = "open";

    /// <summary>
    /// وضعیت پینگ‌ها
    /// </summary>
    public string PingStatus { get; set; } = "open";

    /// <summary>
    /// رمز عبور پست
    /// </summary>
    public string PostPassword { get; set; } = "";

    /// <summary>
    /// نام مستعار پست
    /// </summary>
    public string PostName { get; set; } = "";

    /// <summary>
    /// لینک‌هایی که باید پینگ شوند
    /// </summary>
    public string ToPing { get; set; } = "";

    /// <summary>
    /// لینک‌هایی که پینگ شده‌اند
    /// </summary>
    public string Pinged { get; set; } = "";

    /// <summary>
    /// آخرین تاریخ ویرایش پست
    /// </summary>
    public DateTime PostModified { get; set; } = DateTime.MinValue;

    /// <summary>
    /// آخرین تاریخ ویرایش پست (به زمان GMT)
    /// </summary>
    public DateTime PostModifiedGmt { get; set; } = DateTime.MinValue;

    /// <summary>
    /// فیلتر محتوای پست
    /// </summary>
    public string PostContentFiltered { get; set; } = "";

    /// <summary>
    /// شناسه پست والد
    /// </summary>
    public int PostParent { get; set; } = 0;

    /// <summary>
    /// GUID پست
    /// </summary>
    public string Guid { get; set; } = "";

    /// <summary>
    /// ترتیب منو
    /// </summary>
    public int MenuOrder { get; set; } = 0;

    /// <summary>
    /// نوع پست
    /// </summary>
    public string PostType { get; set; } = "post";

    /// <summary>
    /// نوع MIME ضمیمه
    /// </summary>
    public string PostMimeType { get; set; } = "";

    /// <summary>
    /// تعداد نظرات
    /// </summary>
    public string CommentCount { get; set; } = "0";

    /// <summary>
    /// سطح فیلتر داده‌ها
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<Post> _logger;

    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public Post(IMemoryCache cache, ILogger<Post> logger, AppDbContext context)
    {
        _cache = cache;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// بازیابی شیء پست
    /// </summary>
    public static Post GetInstance(int postId, IMemoryCache cache, AppDbContext context, ILogger<Post> logger)
    {
        if (postId == 0)
        {
            return null;
        }

        var cachedPost = cache.Get<Post>(postId);
        if (cachedPost != null)
        {
            return cachedPost;
        }

        var post = context.Posts.FirstOrDefault(p => p.ID == postId);
        if (post == null)
        {
            return null;
        }

        cache.Set(postId, post, TimeSpan.FromMinutes(10));
        return post;
    }

    /// <summary>
    /// بررسی وجود خصوصیت
    /// </summary>
    public bool IsPropertySet(string key)
    {
        return key switch
        {
            "ancestors" or "page_template" or "post_category" or "tags_input" => true,
            _ => context.PostMeta.Any(pm => pm.PostId == ID && pm.Key == key)
        };
    }

    /// <summary>
    /// دریافت مقدار خصوصیت
    /// </summary>
    public object GetPropertyValue(string key)
    {
        return key switch
        {
            "page_template" => context.PostMeta.FirstOrDefault(pm => pm.PostId == ID && pm.Key == "_wp_page_template")?.Value,
            "post_category" => context.Terms.Where(t => t.PostId == ID && t.Taxonomy == "category").Select(t => t.TermId).ToList(),
            "tags_input" => context.Terms.Where(t => t.PostId == ID && t.Taxonomy == "post_tag").Select(t => t.Name).ToList(),
            "ancestors" => context.Posts.Where(p => p.PostParent == ID).Select(p => p.ID).ToList(),
            _ => context.PostMeta.FirstOrDefault(pm => pm.PostId == ID && pm.Key == key)?.Value
        };
    }

    /// <summary>
    /// فیلتر کردن داده‌ها
    /// </summary>
    public Post Filter(string filter)
    {
        if (Filter == filter)
        {
            return this;
        }

        if (filter == "raw")
        {
            return GetInstance(ID, _cache, _context, _logger);
        }

        // فیلتر کردن داده‌ها (شبیه‌سازی شده)
        return this;
    }

    /// <summary>
    /// تبدیل شیء به آرایه
    /// </summary>
    public Dictionary<string, object> ToArray()
    {
        var post = GetType().GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(this));

        foreach (var key in new[] { "ancestors", "page_template", "post_category", "tags_input" })
        {
            if (IsPropertySet(key))
            {
                post[key] = GetPropertyValue(key);
            }
        }

        return post;
    }
}