using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class Network
{
    /// <summary>
    /// شناسه شبکه
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// دامنه شبکه
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// مسیر شبکه
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// نام شبکه
    /// </summary>
    public string SiteName { get; set; }

    /// <summary>
    /// شناسه سایت اصلی
    /// </summary>
    public int BlogId { get; set; }

    /// <summary>
    /// دامنه مربوط به کوکی‌ها
    /// </summary>
    public string CookieDomain { get; set; }

    /// <summary>
    /// بازیابی شبکه از دیتابیس بر اساس شناسه
    /// </summary>
    public static Network GetInstance(NetworkDbContext context, int networkId)
    {
        if (networkId <= 0)
        {
            return null;
        }

        // بررسی کش
        var cachedNetwork = context.Networks.FirstOrDefault(n => n.Id == networkId);
        if (cachedNetwork != null)
        {
            return cachedNetwork;
        }

        // بازیابی از دیتابیس
        var network = context.Networks
            .Where(n => n.Id == networkId)
            .FirstOrDefault();

        if (network != null)
        {
            // ذخیره در کش
            context.Networks.Add(network);
            context.SaveChanges();
        }

        return network;
    }

    /// <summary>
    /// تعیین شناسه سایت اصلی
    /// </summary>
    public int GetMainSiteId()
    {
        // فیلتر برای تعیین سایت اصلی
        var mainSiteId = 0; // مقدار پیش‌فرض

        if (BlogId > 0)
        {
            mainSiteId = BlogId;
        }
        else if (!string.IsNullOrEmpty(Domain) && !string.IsNullOrEmpty(Path))
        {
            // بررسی تنظیمات فعلی
            if (Domain == Environment.GetEnvironmentVariable("DOMAIN_CURRENT_SITE") &&
                Path == Environment.GetEnvironmentVariable("PATH_CURRENT_SITE"))
            {
                mainSiteId = BlogId;
            }
        }

        return mainSiteId;
    }
}