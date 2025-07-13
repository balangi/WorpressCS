using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class NavMenuService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NavMenuService> _logger;

    public NavMenuService(ApplicationDbContext context, ILogger<NavMenuService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a navigation menu by its ID, slug, or name.
    /// </summary>
    public NavMenu GetNavMenu(object menuIdentifier)
    {
        if (menuIdentifier == null)
        {
            return null;
        }

        return _context.NavMenus
            .Include(m => m.Items)
            .FirstOrDefault(m =>
                m.Id.ToString() == menuIdentifier.ToString() ||
                m.Name.Equals(menuIdentifier.ToString(), StringComparison.OrdinalIgnoreCase) ||
                m.Slug.Equals(menuIdentifier.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Creates a new navigation menu.
    /// </summary>
    public NavMenu CreateNavMenu(string menuName)
    {
        if (string.IsNullOrWhiteSpace(menuName))
        {
            throw new ArgumentException("Menu name cannot be empty.");
        }

        var existingMenu = _context.NavMenus.FirstOrDefault(m => m.Name.Equals(menuName, StringComparison.OrdinalIgnoreCase));
        if (existingMenu != null)
        {
            throw new InvalidOperationException($"A menu with the name '{menuName}' already exists.");
        }

        var newMenu = new NavMenu
        {
            Name = menuName,
            Slug = menuName.ToLowerInvariant().Replace(" ", "-"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.NavMenus.Add(newMenu);
        _context.SaveChanges();
        return newMenu;
    }

    /// <summary>
    /// Deletes a navigation menu.
    /// </summary>
    public bool DeleteNavMenu(int menuId)
    {
        var menu = _context.NavMenus.Include(m => m.Items).FirstOrDefault(m => m.Id == menuId);
        if (menu == null)
        {
            return false;
        }

        foreach (var item in menu.Items)
        {
            _context.NavItems.Remove(item);
        }

        _context.NavMenus.Remove(menu);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Adds a new menu item to a navigation menu.
    /// </summary>
    public NavItem AddMenuItem(int menuId, string title, string url, string objectType, int objectId)
    {
        var menu = _context.NavMenus.Find(menuId);
        if (menu == null)
        {
            throw new InvalidOperationException("Menu not found.");
        }

        var newItem = new NavItem
        {
            MenuId = menuId,
            Title = title,
            Url = url,
            ObjectType = objectType,
            ObjectId = objectId,
            CreatedAt = DateTime.UtcNow
        };

        _context.NavItems.Add(newItem);
        _context.SaveChanges();
        return newItem;
    }

    /// <summary>
    /// Updates an existing menu item.
    /// </summary>
    public NavItem UpdateMenuItem(int itemId, string title, string url)
    {
        var item = _context.NavItems.Find(itemId);
        if (item == null)
        {
            throw new InvalidOperationException("Menu item not found.");
        }

        item.Title = title;
        item.Url = url;
        item.UpdatedAt = DateTime.UtcNow;

        _context.SaveChanges();
        return item;
    }

    /// <summary>
    /// Deletes a menu item.
    /// </summary>
    public bool DeleteMenuItem(int itemId)
    {
        var item = _context.NavItems.Find(itemId);
        if (item == null)
        {
            return false;
        }

        _context.NavItems.Remove(item);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Retrieves all registered navigation menus.
    /// </summary>
    public List<NavMenu> GetAllNavMenus()
    {
        return _context.NavMenus.Include(m => m.Items).ToList();
    }

    /// <summary>
    /// Checks if a given ID is a valid navigation menu.
    /// </summary>
    public bool IsNavMenu(int menuId)
    {
        return _context.NavMenus.Any(m => m.Id == menuId);
    }
}