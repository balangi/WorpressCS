using System;
using System.Linq;

public class SiteService
{
    private readonly AppDbContext _context;

    public SiteService(AppDbContext context)
    {
        _context = context;
    }

    // دریافت یک سایت بر اساس شناسه
    public Site GetSiteById(int siteId)
    {
        var site = _context.Sites
            .Include(s => s.Details) // Lazy Loading اطلاعات اضافی
            .FirstOrDefault(s => s.BlogId == siteId);

        return site;
    }

    // ایجاد یک سایت جدید
    public void CreateSite(Site site)
    {
        _context.Sites.Add(site);
        _context.SaveChanges();
    }

    // به‌روزرسانی اطلاعات یک سایت
    public void UpdateSite(Site site)
    {
        _context.Sites.Update(site);
        _context.SaveChanges();
    }

    // حذف یک سایت
    public void DeleteSite(int siteId)
    {
        var site = _context.Sites.FirstOrDefault(s => s.BlogId == siteId);
        if (site != null)
        {
            _context.Sites.Remove(site);
            _context.SaveChanges();
        }
    }

    // دریافت اطلاعات اضافی یک سایت
    public SiteDetails GetSiteDetails(int siteId)
    {
        var details = _context.SiteDetails.FirstOrDefault(d => d.BlogId == siteId);
        return details;
    }

    // تنظیم اطلاعات اضافی یک سایت
    public void SetSiteDetails(int siteId, SiteDetails details)
    {
        var existingDetails = _context.SiteDetails.FirstOrDefault(d => d.BlogId == siteId);
        if (existingDetails != null)
        {
            existingDetails.BlogName = details.BlogName;
            existingDetails.SiteUrl = details.SiteUrl;
            existingDetails.PostCount = details.PostCount;
            existingDetails.Home = details.Home;

            _context.SaveChanges();
        }
    }
}