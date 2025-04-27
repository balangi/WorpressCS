using System;
using System.Collections.Generic;
using System.Linq;

public class CustomizePanelManager
{
    private readonly AppDbContext _context;

    // رویدادها
    public event Action<Panel> OnPanelAdded;
    public event Action<int> OnPanelRemoved;

    public CustomizePanelManager(AppDbContext context)
    {
        _context = context;
    }

    // افزودن پنل جدید
    public void AddPanel(string title, string description, string type, string priority)
    {
        var panel = new Panel
        {
            Title = title,
            Description = description,
            Type = type,
            Priority = priority,
            Settings = new List<PanelSetting>()
        };

        _context.Panels.Add(panel);
        _context.SaveChanges();

        OnPanelAdded?.Invoke(panel);
    }

    // دریافت پنل بر اساس شناسه
    public Panel GetPanel(int id)
    {
        return _context.Panels.Include(p => p.Settings).FirstOrDefault(p => p.Id == id);
    }

    // حذف پنل
    public void RemovePanel(int id)
    {
        var panel = _context.Panels.FirstOrDefault(p => p.Id == id);
        if (panel != null)
        {
            _context.Panels.Remove(panel);
            _context.SaveChanges();

            OnPanelRemoved?.Invoke(id);
        }
    }

    // افزودن تنظیم به پنل
    public void AddPanelSetting(int panelId, string key, string value)
    {
        var panel = _context.Panels.FirstOrDefault(p => p.Id == panelId);
        if (panel != null)
        {
            var setting = new PanelSetting
            {
                Key = key,
                Value = value,
                PanelId = panelId
            };

            panel.Settings.Add(setting);
            _context.SaveChanges();
        }
    }

    // دریافت تمام پنل‌ها
    public IEnumerable<Panel> GetAllPanels()
    {
        return _context.Panels.Include(p => p.Settings).ToList();
    }
}