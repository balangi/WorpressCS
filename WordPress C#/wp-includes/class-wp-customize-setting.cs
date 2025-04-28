using System;
using System.Collections.Generic;
using System.Linq;

public class CustomizeSettingManager
{
    private readonly AppDbContext _context;

    // رویدادها
    public event Action<CustomSetting> OnSettingAdded;
    public event Action<int> OnSettingRemoved;
    public event Action<CustomSetting> OnSettingUpdated;

    public CustomizeSettingManager(AppDbContext context)
    {
        _context = context;
    }

    // افزودن تنظیم جدید
    public void AddSetting(string key, string value, string type, string defaultValue)
    {
        var setting = new CustomSetting
        {
            Key = key,
            Value = value,
            Type = type,
            DefaultValue = defaultValue,
            IsDirty = false
        };

        _context.CustomSettings.Add(setting);
        _context.SaveChanges();

        OnSettingAdded?.Invoke(setting);
    }

    // دریافت تنظیم بر اساس کلید
    public CustomSetting GetSetting(string key)
    {
        return _context.CustomSettings.FirstOrDefault(s => s.Key == key);
    }

    // حذف تنظیم
    public void RemoveSetting(string key)
    {
        var setting = _context.CustomSettings.FirstOrDefault(s => s.Key == key);
        if (setting != null)
        {
            _context.CustomSettings.Remove(setting);
            _context.SaveChanges();

            OnSettingRemoved?.Invoke(setting.Id);
        }
    }

    // به‌روزرسانی تنظیم
    public void UpdateSetting(string key, string newValue)
    {
        var setting = _context.CustomSettings.FirstOrDefault(s => s.Key == key);
        if (setting != null)
        {
            setting.Value = newValue;
            setting.IsDirty = true;

            _context.SaveChanges();

            OnSettingUpdated?.Invoke(setting);
        }
    }

    // ذخیره تمام تنظیمات
    public void Save()
    {
        foreach (var setting in _context.CustomSettings.Where(s => s.IsDirty))
        {
            setting.IsDirty = false;
        }
        _context.SaveChanges();
    }

    // دریافت تمام تنظیمات
    public IEnumerable<CustomSetting> GetAllSettings()
    {
        return _context.CustomSettings.ToList();
    }
}