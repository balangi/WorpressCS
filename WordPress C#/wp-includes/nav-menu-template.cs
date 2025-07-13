using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class NavMenuTemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NavMenuTemplateService> _logger;

    public NavMenuTemplateService(ApplicationDbContext context, ILogger<NavMenuTemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Displays a navigation menu.
    /// </summary>
    public string DisplayNavMenu(int? menuId, NavMenuArgs args = null)
    {
        if (args == null)
        {
            args = new NavMenuArgs();
        }

        var menu = GetNavMenu(menuId);
        if (menu == null || !menu.Items.Any())
        {
            return FallbackCallback(args);
        }

        var sortedItems = SortMenuItems(menu.Items);
        var htmlBuilder = new StringBuilder();

        if (!string.IsNullOrEmpty(args.Container))
        {
            htmlBuilder.Append($"<{args.Container} id=\"{args.ContainerId}\" class=\"{args.ContainerClass}\">");
        }

        htmlBuilder.Append("<ul>");
        foreach (var item in sortedItems)
        {
            BuildMenuItemHtml(item, htmlBuilder, args);
        }
        htmlBuilder.Append("</ul>");

        if (!string.IsNullOrEmpty(args.Container))
        {
            htmlBuilder.Append($"</{args.Container}>");
        }

        return htmlBuilder.ToString();
    }

    /// <summary>
    /// Retrieves a navigation menu by its ID or slug.
    /// </summary>
    private NavMenu GetNavMenu(int? menuId)
    {
        return _context.NavMenus
            .Include(m => m.Items)
            .FirstOrDefault(m => m.Id == menuId || m.Slug.Equals(menuId.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Sorts menu items based on their order and hierarchy.
    /// </summary>
    private List<NavItem> SortMenuItems(List<NavItem> items)
    {
        var sortedItems = new List<NavItem>();
        var topLevelItems = items.Where(i => i.ParentItemId == 0).OrderBy(i => i.MenuOrder).ToList();

        foreach (var item in topLevelItems)
        {
            sortedItems.Add(item);
            AddChildItems(item, items, sortedItems);
        }

        return sortedItems;
    }

    /// <summary>
    /// Recursively adds child items to the sorted list.
    /// </summary>
    private void AddChildItems(NavItem parent, List<NavItem> allItems, List<NavItem> sortedItems)
    {
        var children = allItems.Where(i => i.ParentItemId == parent.Id).OrderBy(i => i.MenuOrder).ToList();
        foreach (var child in children)
        {
            sortedItems.Add(child);
            AddChildItems(child, allItems, sortedItems);
        }
    }

    /// <summary>
    /// Builds the HTML for a single menu item.
    /// </summary>
    private void BuildMenuItemHtml(NavItem item, StringBuilder htmlBuilder, NavMenuArgs args)
    {
        htmlBuilder.Append("<li>");
        htmlBuilder.Append($"<a href=\"{item.Url}\" target=\"{item.Target}\">{item.Title}</a>");

        if (item.ChildItems.Any())
        {
            htmlBuilder.Append("<ul>");
            foreach (var child in item.ChildItems.OrderBy(c => c.MenuOrder))
            {
                BuildMenuItemHtml(child, htmlBuilder, args);
            }
            htmlBuilder.Append("</ul>");
        }

        htmlBuilder.Append("</li>");
    }

    /// <summary>
    /// Fallback callback when no menu is found.
    /// </summary>
    private string FallbackCallback(NavMenuArgs args)
    {
        if (args.FallbackCallback != null && args.FallbackCallback is Func<string> fallback)
        {
            return fallback();
        }

        return "<p>No menu found.</p>";
    }
}

/// <summary>
/// Represents arguments for displaying a navigation menu.
/// </summary>
public class NavMenuArgs
{
    public string Container { get; set; } = "div";
    public string ContainerId { get; set; }
    public string ContainerClass { get; set; }
    public string ContainerAriaLabel { get; set; }
    public string Before { get; set; }
    public string After { get; set; }
    public string LinkBefore { get; set; }
    public string LinkAfter { get; set; }
    public Func<string> FallbackCallback { get; set; }
}