using System;
using System.Collections.Generic;
using System.Linq;

public class CustomizeWidgetManager
{
    private readonly AppDbContext _context;

    // رویدادها
    public event Action<Widget> OnWidgetAdded;
    public event Action<int> OnWidgetRemoved;

    public CustomizeWidgetManager(AppDbContext context)
    {
        _context = context;
    }

    // افزودن ویجت جدید
    public void AddWidget(string name, string type, string sidebarId)
    {
        var widget = new Widget
        {
            Name = name,
            Type = type,
            SidebarId = sidebarId,
            Settings = new List<WidgetSetting>()
        };

        _context.Widgets.Add(widget);
        _context.SaveChanges();

        OnWidgetAdded?.Invoke(widget);
    }

    // دریافت ویجت بر اساس شناسه
    public Widget GetWidget(int id)
    {
        return _context.Widgets.Include(w => w.Settings).FirstOrDefault(w => w.Id == id);
    }

    // حذف ویجت
    public void RemoveWidget(int id)
    {
        var widget = _context.Widgets.FirstOrDefault(w => w.Id == id);
        if (widget != null)
        {
            _context.Widgets.Remove(widget);
            _context.SaveChanges();

            OnWidgetRemoved?.Invoke(id);
        }
    }

    // افزودن تنظیم به ویجت
    public void AddWidgetSetting(int widgetId, string key, string value)
    {
        var widget = _context.Widgets.FirstOrDefault(w => w.Id == widgetId);
        if (widget != null)
        {
            var setting = new WidgetSetting
            {
                Key = key,
                Value = value,
                WidgetId = widgetId
            };

            widget.Settings.Add(setting);
            _context.SaveChanges();
        }
    }

    // دریافت تمام ویجت‌ها
    public IEnumerable<Widget> GetAllWidgets()
    {
        return _context.Widgets.Include(w => w.Settings).ToList();
    }
}