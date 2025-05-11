using System;
using System.Collections.Generic;
using System.Linq;

public class TermQueryService
{
    private readonly AppDbContext _context;

    public TermQueryService(AppDbContext context)
    {
        _context = context;
    }

    // اجرای Term Query
    public IQueryable<Term> ExecuteTermQuery(TermQueryParameters parameters)
    {
        var query = _context.Terms.AsQueryable();

        // فیلتر بر اساس Taxonomy
        if (!string.IsNullOrEmpty(parameters.Taxonomy))
        {
            query = query.Where(t => t.Taxonomies.Any(tt => tt.Taxonomy == parameters.Taxonomy));
        }

        // فیلتر بر اساس شناسه‌ها
        if (parameters.Include != null && parameters.Include.Any())
        {
            query = query.Where(t => parameters.Include.Contains(t.TermId));
        }

        // فیلتر بر اساس Exclude
        if (parameters.Exclude != null && parameters.Exclude.Any())
        {
            query = query.Where(t => !parameters.Exclude.Contains(t.TermId));
        }

        // فیلتر بر اساس Slug
        if (!string.IsNullOrEmpty(parameters.Slug))
        {
            query = query.Where(t => t.Slug == parameters.Slug);
        }

        // فیلتر بر اساس نام
        if (!string.IsNullOrEmpty(parameters.Name))
        {
            query = query.Where(t => t.Name.Contains(parameters.Name));
        }

        // مرتب‌سازی
        if (!string.IsNullOrEmpty(parameters.OrderBy))
        {
            switch (parameters.OrderBy.ToLower())
            {
                case "name":
                    query = parameters.Order == "asc" ? query.OrderBy(t => t.Name) : query.OrderByDescending(t => t.Name);
                    break;
                case "slug":
                    query = parameters.Order == "asc" ? query.OrderBy(t => t.Slug) : query.OrderByDescending(t => t.Slug);
                    break;
                default:
                    query = query.OrderBy(t => t.TermId); // مرتب‌سازی پیش‌فرض بر اساس شناسه
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

    // دریافت تعداد کل Term‌ها
    public int GetTotalTermsCount(TermQueryParameters parameters)
    {
        return ExecuteTermQuery(parameters).Count();
    }

    // دریافت لیست Term‌ها
    public List<Term> GetTerms(TermQueryParameters parameters)
    {
        return ExecuteTermQuery(parameters).ToList();
    }
}