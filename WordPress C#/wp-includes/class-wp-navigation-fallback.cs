using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class NavigationFallback
{
    /// <summary>
    /// به‌روزرسانی Schema برای wp_navigation
    /// </summary>
    public static Dictionary<string, object> UpdateWpNavigationPostSchema(Dictionary<string, object> schema)
    {
        // افزودن فیلدهای بالایی به Context
        if (schema.ContainsKey("status"))
        {
            schema["status"]["context"] = ((List<string>)schema["status"]["context"]).Concat(new[] { "embed" }).ToList();
        }

        if (schema.ContainsKey("content"))
        {
            schema["content"]["context"] = ((List<string>)schema["content"]["context"]).Concat(new[] { "embed" }).ToList();

            // افزودن زیرفیلدهای content
            if (schema["content"].ContainsKey("properties"))
            {
                var contentProperties = (Dictionary<string, object>)schema["content"]["properties"];
                if (contentProperties.ContainsKey("raw"))
                {
                    contentProperties["raw"]["context"] = ((List<string>)contentProperties["raw"]["context"]).Concat(new[] { "embed" }).ToList();
                }
                if (contentProperties.ContainsKey("rendered"))
                {
                    contentProperties["rendered"]["context"] = ((List<string>)contentProperties["rendered"]["context"]).Concat(new[] { "embed" }).ToList();
                }
                if (contentProperties.ContainsKey("block_version"))
                {
                    contentProperties["block_version"]["context"] = ((List<string>)contentProperties["block_version"]["context"]).Concat(new[] { "embed" }).ToList();
                }
            }
        }

        // افزودن زیرفیلدهای title
        if (schema.ContainsKey("title") && schema["title"].ContainsKey("properties"))
        {
            var titleProperties = (Dictionary<string, object>)schema["title"]["properties"];
            if (titleProperties.ContainsKey("raw"))
            {
                titleProperties["raw"]["context"] = ((List<string>)titleProperties["raw"]["context"]).Concat(new[] { "embed" }).ToList();
            }
        }

        return schema;
    }

    /// <summary>
    /// دریافت یا ایجاد یک منوی پشتیبان
    /// </summary>
    public static NavigationMenu GetFallback(NavigationDbContext context)
    {
        var shouldCreateFallback = true; // فیلتر کردن با استفاده از Dependency Injection یا تنظیمات

        var fallback = GetMostRecentlyPublishedNavigation(context);

        if (fallback != null || !shouldCreateFallback)
        {
            return fallback;
        }

        fallback = CreateClassicMenuFallback(context);

        if (fallback != null && !(fallback is Exception))
        {
            return fallback;
        }

        fallback = CreateDefaultFallback(context);

        if (fallback != null && !(fallback is Exception))
        {
            return fallback;
        }

        return null;
    }

    /// <summary>
    /// دریافت آخرین منوی منتشرشده
    /// </summary>
    private static NavigationMenu GetMostRecentlyPublishedNavigation(NavigationDbContext context)
    {
        return context.NavigationMenus
            .Where(n => n.Status == "publish")
            .OrderByDescending(n => n.PublishDate)
            .FirstOrDefault();
    }

    /// <summary>
    /// ایجاد یک منوی پشتیبان از منوی کلاسیک
    /// </summary>
    private static NavigationMenu CreateClassicMenuFallback(NavigationDbContext context)
    {
        var classicMenu = GetFallbackClassicMenu(context);

        if (classicMenu == null)
        {
            throw new InvalidOperationException("No Classic Menus found.");
        }

        var classicMenuBlocks = ConvertClassicMenuToBlocks(classicMenu);

        if (string.IsNullOrEmpty(classicMenuBlocks))
        {
            throw new InvalidOperationException("Unable to convert Classic Menu to blocks.");
        }

        var newMenu = new NavigationMenu
        {
            Content = classicMenuBlocks,
            Title = classicMenu.Name,
            Slug = classicMenu.Slug,
            Status = "publish",
            Type = "wp_navigation"
        };

        context.NavigationMenus.Add(newMenu);
        context.SaveChanges();

        return newMenu;
    }

    /// <summary>
    /// دریافت منوی کلاسیک پشتیبان
    /// </summary>
    private static ClassicMenu GetFallbackClassicMenu(NavigationDbContext context)
    {
        var classicMenus = context.ClassicMenus.ToList();

        if (classicMenus == null || classicMenus.Count == 0)
        {
            return null;
        }

        var primaryMenu = GetNavMenuAtPrimaryLocation(context);

        if (primaryMenu != null)
        {
            return primaryMenu;
        }

        var primarySlugMenu = GetNavMenuWithPrimarySlug(classicMenus);

        if (primarySlugMenu != null)
        {
            return primarySlugMenu;
        }

        return GetMostRecentlyCreatedNavMenu(classicMenus);
    }

    /// <summary>
    /// ایجاد یک منوی پشتیبان پیش‌فرض
    /// </summary>
    private static NavigationMenu CreateDefaultFallback(NavigationDbContext context)
    {
        var defaultBlocks = GetDefaultFallbackBlocks();

        var newMenu = new NavigationMenu
        {
            Content = defaultBlocks,
            Title = "Navigation",
            Slug = "navigation",
            Status = "publish",
            Type = "wp_navigation"
        };

        context.NavigationMenus.Add(newMenu);
        context.SaveChanges();

        return newMenu;
    }

    /// <summary>
    /// دریافت بلوک‌های پیش‌فرض
    /// </summary>
    private static string GetDefaultFallbackBlocks()
    {
        // بررسی ثبت بلوک‌ها
        return "<!-- wp:page-list /-->";
    }
}