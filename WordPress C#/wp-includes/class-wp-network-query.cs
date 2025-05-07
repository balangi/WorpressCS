using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class NetworkQuery
{
    /// <summary>
    /// پرس‌وجوی فعلی
    /// </summary>
    public Dictionary<string, object> QueryVars { get; private set; }

    /// <summary>
    /// مقادیر پیش‌فرض پرس‌وجو
    /// </summary>
    private readonly Dictionary<string, object> _queryVarDefaults = new()
    {
        { "network__in", Array.Empty<int>() },
        { "network__not_in", Array.Empty<int>() },
        { "count", false },
        { "fields", string.Empty },
        { "number", null },
        { "offset", 0 },
        { "no_found_rows", true },
        { "orderby", "id" },
        { "order", "ASC" },
        { "domain", string.Empty },
        { "domain__in", Array.Empty<string>() },
        { "path", string.Empty },
        { "search", string.Empty }
    };

    /// <summary>
    /// لیست شبکه‌های یافت‌شده
    /// </summary>
    public List<Network> Networks { get; private set; }

    /// <summary>
    /// تعداد شبکه‌های یافت‌شده
    /// </summary>
    public int FoundNetworks { get; private set; }

    /// <summary>
    /// تعداد صفحات
    /// </summary>
    public int MaxNumPages { get; private set; }

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public NetworkQuery(Dictionary<string, object> query = null)
    {
        QueryVars = query != null ? MergeQueryVars(query) : _queryVarDefaults;
    }

    /// <summary>
    /// ادغام پارامترهای پرس‌وجو با مقادیر پیش‌فرض
    /// </summary>
    private Dictionary<string, object> MergeQueryVars(Dictionary<string, object> query)
    {
        var mergedQuery = new Dictionary<string, object>(_queryVarDefaults);
        foreach (var key in query.Keys)
        {
            if (mergedQuery.ContainsKey(key))
            {
                mergedQuery[key] = query[key];
            }
        }
        return mergedQuery;
    }

    /// <summary>
    /// اجرای پرس‌وجو
    /// </summary>
    public List<Network> GetNetworks(NetworkDbContext context)
    {
        ParseQuery();
        PreGetNetworks();

        var networkData = ApplyFilters(context);

        if (networkData != null)
        {
            Networks = networkData;
            return Networks;
        }

        Networks = ExecuteQuery(context);
        return Networks;
    }

    /// <summary>
    /// پردازش پارامترهای جستجو
    /// </summary>
    private void ParseQuery()
    {
        // پردازش پارامترهای جستجو
        // مثال: بررسی مقادیر عددی و رشته‌ای
    }

    /// <summary>
    /// اعمال فیلترها قبل از اجرای پرس‌وجو
    /// </summary>
    private void PreGetNetworks()
    {
        // اعمال فیلترهای قبل از جستجو
    }

    /// <summary>
    /// اعمال فیلترها برای جلوگیری از اجرای پرس‌وجو
    /// </summary>
    private List<Network> ApplyFilters(NetworkDbContext context)
    {
        // شبیه‌سازی فیلترهای WordPress
        return null;
    }

    /// <summary>
    /// اجرای پرس‌وجو با LINQ
    /// </summary>
    private List<Network> ExecuteQuery(NetworkDbContext context)
    {
        var query = context.Networks.AsQueryable();

        // فیلترها
        if (QueryVars.ContainsKey("network__in") && QueryVars["network__in"] is IEnumerable<int> networkIn)
        {
            query = query.Where(n => networkIn.Contains(n.Id));
        }

        if (QueryVars.ContainsKey("network__not_in") && QueryVars["network__not_in"] is IEnumerable<int> networkNotIn)
        {
            query = query.Where(n => !networkNotIn.Contains(n.Id));
        }

        if (QueryVars.ContainsKey("domain") && QueryVars["domain"] is string domain)
        {
            query = query.Where(n => n.Domain == domain);
        }

        if (QueryVars.ContainsKey("search") && QueryVars["search"] is string search)
        {
            query = query.Where(n => n.Domain.Contains(search) || n.Path.Contains(search));
        }

        // مرتب‌سازی
        if (QueryVars.ContainsKey("orderby") && QueryVars["orderby"] is string orderby)
        {
            query = orderby switch
            {
                "id" => QueryVars["order"].ToString() == "ASC"
                    ? query.OrderBy(n => n.Id)
                    : query.OrderByDescending(n => n.Id),
                "domain" => QueryVars["order"].ToString() == "ASC"
                    ? query.OrderBy(n => n.Domain)
                    : query.OrderByDescending(n => n.Domain),
                _ => query
            };
        }

        // محدودیت‌ها
        if (QueryVars.ContainsKey("number") && QueryVars["number"] is int number)
        {
            query = query.Take(number);
        }

        if (QueryVars.ContainsKey("offset") && QueryVars["offset"] is int offset)
        {
            query = query.Skip(offset);
        }

        return query.ToList();
    }
}