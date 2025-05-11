using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class SiteQueryService
{
    private readonly AppDbContext _context;

    public SiteQueryService(AppDbContext context)
    {
        _context = context;
    }

    // پرس‌وجوی سایت‌ها بر اساس پارامترها
    public IQueryable<Site> QuerySites(SiteQueryParameters parameters)
    {
        var query = _context.Sites.AsQueryable();

        // فیلتر بر اساس شناسه سایت
        if (parameters.SiteIds != null && parameters.SiteIds.Any())
        {
            query = query.Where(s => parameters.SiteIds.Contains(s.BlogId));
        }

        // فیلتر بر اساس دامنه
        if (!string.IsNullOrEmpty(parameters.Domain))
        {
            query = query.Where(s => s.Domain.Contains(parameters.Domain));
        }

        // فیلتر بر اساس مسیر
        if (!string.IsNullOrEmpty(parameters.Path))
        {
            query = query.Where(s => s.Path.Contains(parameters.Path));
        }

        // فیلتر بر اساس وضعیت‌ها
        if (parameters.IsPublic.HasValue)
        {
            query = query.Where(s => s.IsPublic == parameters.IsPublic.Value);
        }
        if (parameters.IsArchived.HasValue)
        {
            query = query.Where(s => s.IsArchived == parameters.IsArchived.Value);
        }
        if (parameters.IsSpam.HasValue)
        {
            query = query.Where(s => s.IsSpam == parameters.IsSpam.Value);
        }

        // مرتب‌سازی
        if (!string.IsNullOrEmpty(parameters.OrderBy))
        {
            switch (parameters.OrderBy.ToLower())
            {
                case "domain":
                    query = parameters.Order == "asc" ? query.OrderBy(s => s.Domain) : query.OrderByDescending(s => s.Domain);
                    break;
                case "path":
                    query = parameters.Order == "asc" ? query.OrderBy(s => s.Path) : query.OrderByDescending(s => s.Path);
                    break;
                default:
                    query = query.OrderBy(s => s.BlogId); // مرتب‌سازی پیش‌فرض بر اساس شناسه
                    break;
            }
        }

        // محدود کردن تعداد نتایج
        if (parameters.Limit > 0)
        {
            query = query.Take(parameters.Limit);
        }

        return query;
    }

    // دریافت تعداد کل سایت‌ها
    public int GetTotalSitesCount(SiteQueryParameters parameters)
    {
        return QuerySites(parameters).Count();
    }

    // دریافت لیست سایت‌ها
    public List<Site> GetSites(SiteQueryParameters parameters)
    {
        return QuerySites(parameters).ToList();
    }
}