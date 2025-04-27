using System;
using System.Collections.Generic;
using System.Linq;

public class CustomizeNavMenus
{
    private readonly AppDbContext _context;

    // رویدادها
    public event Action<Menu> OnMenuAdded;
    public event Action<int> OnMenuRemoved;

    public CustomizeNavMenus(AppDbContext context)
    {
        _context = context;
    }

    // افزودن منو جدید
    public void AddMenu(string name, string slug)
    {
        var menu = new Menu
        {
            Name = name,
            Slug = slug,
            Items = new List<MenuItem>()
        };

        _context.Menus.Add(menu);
        _context.SaveChanges();

        OnMenuAdded?.Invoke(menu);
    }

    // دریافت منو بر اساس شناسه
    public Menu GetMenu(int id)
    {
        return _context.Menus.Include(m => m.Items).FirstOrDefault(m => m.Id == id);
    }

    // حذف منو
    public void RemoveMenu(int id)
    {
        var menu = _context.Menus.FirstOrDefault(m => m.Id == id);
        if (menu != null)
        {
            _context.Menus.Remove(menu);
            _context.SaveChanges();

            OnMenuRemoved?.Invoke(id);
        }
    }

    // افزودن آیتم به منو
    public void AddMenuItem(int menuId, string title, string url, int order)
    {
        var menu = _context.Menus.FirstOrDefault(m => m.Id == menuId);
        if (menu != null)
        {
            var item = new MenuItem
            {
                Title = title,
                Url = url,
                Order = order,
                MenuId = menuId
            };

            menu.Items.Add(item);
            _context.SaveChanges();
        }
    }

    // دریافت تمام منوها
    public IEnumerable<Menu> GetAllMenus()
    {
        return _context.Menus.Include(m => m.Items).ToList();
    }
}