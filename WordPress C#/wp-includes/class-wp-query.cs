using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class PostQuery
{
    /// <summary>
    /// پارامترهای پرس‌وجو
    /// </summary>
    public Dictionary<string, object> QueryVars { get; set; } = new();

    /// <summary>
    /// لیست پست‌ها
    /// </summary>
    public List<Post> Posts { get; set; } = new();

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<PostQuery> _logger;

    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public PostQuery(IMemoryCache cache, ILogger<PostQuery> logger, AppDbContext context)
    {
        _cache = cache;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// تنظیم پارامترهای پرس‌وجو
    /// </summary>
    public void ParseQuery(Dictionary<string, object> query)
    {
        foreach (var key in query.Keys)
        {
            QueryVars[key] = query[key];
        }
    }

    /// <summary>
    /// اجرای پرس‌وجو
    /// </summary>
    public List<Post> GetPosts()
    {
        var cacheKey = GenerateCacheKey();
        if (_cache.TryGetValue(cacheKey, out List<Post> cachedPosts))
        {
            return cachedPosts;
        }

        var query = _context.Posts.AsQueryable();

        // فیلتر کردن بر اساس پارامترها
        if (QueryVars.ContainsKey("post_type"))
        {
            query = query.Where(p => p.PostType == QueryVars["post_type"].ToString());
        }

        if (QueryVars.ContainsKey("category"))
        {
            query = query.Where(p => p.Categories.Any(c => c.Name == QueryVars["category"].ToString()));
        }

        if (QueryVars.ContainsKey("posts_per_page"))
        {
            query = query.Take(int.Parse(QueryVars["posts_per_page"].ToString()));
        }

        Posts = query.ToList();

        // ذخیره در Cache
        _cache.Set(cacheKey, Posts, TimeSpan.FromMinutes(10));

        return Posts;
    }

    /// <summary>
    /// بررسی وضعیت 404
    /// </summary>
    public bool Is404()
    {
        return !Posts.Any();
    }

    /// <summary>
    /// بررسی اینکه آیا پرس‌وجوی فعلی پرس‌وجوی اصلی است
    /// </summary>
    public bool IsMainQuery()
    {
        return this == _context.MainQuery;
    }

    /// <summary>
    /// تنظیم داده‌های پست برای حلقه
    /// </summary>
    public void SetupPostData(Post post)
    {
        _context.CurrentPost = post;
    }

    /// <summary>
    /// تولید کلید Cache
    /// </summary>
    private string GenerateCacheKey()
    {
        var keyParts = QueryVars.Select(kvp => $"{kvp.Key}={kvp.Value}");
        return string.Join("&", keyParts);
    }
}

/// <summary>
/// مدل داده‌ای پست
/// </summary>
public class Post
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string PostType { get; set; }
    public List<Category> Categories { get; set; } = new();
}

/// <summary>
/// مدل داده‌ای دسته‌بندی
/// </summary>
public class Category
{
    public int ID { get; set; }
    public string Name { get; set; }
}

/// <summary>
/// Database Context
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }

    public PostQuery MainQuery { get; set; }
    public Post CurrentPost { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}