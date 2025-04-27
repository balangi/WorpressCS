using System;
using System.Collections.Generic;
using System.Linq;

public class CustomizeSectionManager
{
    private readonly AppDbContext _context;

    // رویدادها
    public event Action<Section> OnSectionAdded;
    public event Action<int> OnSectionRemoved;

    public CustomizeSectionManager(AppDbContext context)
    {
        _context = context;
    }

    // افزودن بخش جدید
    public void AddSection(string title, string description, string type, string priority)
    {
        var section = new Section
        {
            Title = title,
            Description = description,
            Type = type,
            Priority = priority,
            Settings = new List<SectionSetting>()
        };

        _context.Sections.Add(section);
        _context.SaveChanges();

        OnSectionAdded?.Invoke(section);
    }

    // دریافت بخش بر اساس شناسه
    public Section GetSection(int id)
    {
        return _context.Sections.Include(s => s.Settings).FirstOrDefault(s => s.Id == id);
    }

    // حذف بخش
    public void RemoveSection(int id)
    {
        var section = _context.Sections.FirstOrDefault(s => s.Id == id);
        if (section != null)
        {
            _context.Sections.Remove(section);
            _context.SaveChanges();

            OnSectionRemoved?.Invoke(id);
        }
    }

    // افزودن تنظیم به بخش
    public void AddSectionSetting(int sectionId, string key, string value)
    {
        var section = _context.Sections.FirstOrDefault(s => s.Id == sectionId);
        if (section != null)
        {
            var setting = new SectionSetting
            {
                Key = key,
                Value = value,
                SectionId = sectionId
            };

            section.Settings.Add(setting);
            _context.SaveChanges();
        }
    }

    // دریافت تمام بخش‌ها
    public IEnumerable<Section> GetAllSections()
    {
        return _context.Sections.Include(s => s.Settings).ToList();
    }
}