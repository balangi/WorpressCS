using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class DateQuery
{
    private readonly List<Expression<Func<Post, bool>>> _conditions = new();

    // افزودن شرط "بعد از" (After)
    public void AddAfter(DateTime date, bool inclusive = false)
    {
        if (inclusive)
        {
            _conditions.Add(post => post.PublishedDate >= date);
        }
        else
        {
            _conditions.Add(post => post.PublishedDate > date);
        }
    }

    // افزودن شرط "قبل از" (Before)
    public void AddBefore(DateTime date, bool inclusive = false)
    {
        if (inclusive)
        {
            _conditions.Add(post => post.PublishedDate <= date);
        }
        else
        {
            _conditions.Add(post => post.PublishedDate < date);
        }
    }

    // افزودن شرط بین دو تاریخ (Between)
    public void AddBetween(DateTime startDate, DateTime endDate, bool inclusive = false)
    {
        if (inclusive)
        {
            _conditions.Add(post => post.PublishedDate >= startDate && post.PublishedDate <= endDate);
        }
        else
        {
            _conditions.Add(post => post.PublishedDate > startDate && post.PublishedDate < endDate);
        }
    }

    // ساخت پرس‌وجو نهایی
    public IQueryable<Post> BuildQuery(IQueryable<Post> query)
    {
        foreach (var condition in _conditions)
        {
            query = query.Where(condition);
        }
        return query;
    }
}