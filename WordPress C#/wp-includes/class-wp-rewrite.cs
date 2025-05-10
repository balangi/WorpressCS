using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RewriteService
{
    /// <summary>
    /// لیست قوانین بازنویسی
    /// </summary>
    private Dictionary<string, string> _rules = new();

    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<RewriteService> _logger;

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public RewriteService(AppDbContext context, ILogger<RewriteService> logger, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// تولید قوانین بازنویسی
    /// </summary>
    public void GenerateRewriteRules()
    {
        var rules = new Dictionary<string, string>();

        // مثال: اضافه کردن قوانین پایه
        rules.Add("^index\\.php$", "- [L]");
        rules.Add("^robots\\.txt$", "index.php?robots=1");
        rules.Add("^sitemap\\.xml$", "index.php?sitemap=1");

        // ذخیره قوانین
        _rules = rules;
        SaveRewriteRules();
    }

    /// <summary>
    /// ذخیره قوانین بازنویسی
    /// </summary>
    private void SaveRewriteRules()
    {
        _cache.Set("rewrite_rules", _rules, TimeSpan.FromHours(1));
        _context.RewriteRules.UpdateRange(_rules.Select(r => new RewriteRule
        {
            Pattern = r.Key,
            Replacement = r.Value
        }));
        _context.SaveChanges();
    }

    /// <summary>
    /// بازیابی قوانین بازنویسی
    /// </summary>
    public Dictionary<string, string> GetRewriteRules()
    {
        if (_cache.TryGetValue("rewrite_rules", out Dictionary<string, string> cachedRules))
        {
            return cachedRules;
        }

        var rules = _context.RewriteRules.ToDictionary(r => r.Pattern, r => r.Replacement);
        _cache.Set("rewrite_rules", rules, TimeSpan.FromHours(1));
        return rules;
    }

    /// <summary>
    /// اضافه کردن قانون خارجی
    /// </summary>
    public void AddExternalRule(string pattern, string replacement)
    {
        _rules[pattern] = replacement;
        SaveRewriteRules();
    }

    /// <summary>
    /// اضافه کردن Endpoint
    /// </summary>
    public void AddRewriteEndpoint(string name, string queryVar)
    {
        _rules[$"^{name}/(.*)$"] = $"index.php?{queryVar}=$1";
        SaveRewriteRules();
    }

    /// <summary>
    /// تمیز کردن قوانین بازنویسی
    /// </summary>
    public void FlushRules(bool hard = true)
    {
        _rules.Clear();
        _context.RewriteRules.RemoveRange(_context.RewriteRules);
        _context.SaveChanges();

        if (hard)
        {
            // اعمال تغییرات در فایل .htaccess (در صورت نیاز)
            WriteToHtaccess();
        }
    }

    /// <summary>
    /// نوشتن قوانین در فایل .htaccess
    /// </summary>
    private void WriteToHtaccess()
    {
        var htaccessContent = "<IfModule mod_rewrite.c>\n";
        htaccessContent += "RewriteEngine On\n";

        foreach (var rule in _rules)
        {
            htaccessContent += $"RewriteRule {rule.Key} {rule.Value} [L]\n";
        }

        htaccessContent += "</IfModule>";

        System.IO.File.WriteAllText(".htaccess", htaccessContent);
    }
}

/// <summary>
/// مدل داده‌ای قوانین بازنویسی
/// </summary>
public class RewriteRule
{
    public int Id { get; set; }
    public string Pattern { get; set; }
    public string Replacement { get; set; }
}

/// <summary>
/// Database Context
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<RewriteRule> RewriteRules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}