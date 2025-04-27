public class CustomSettingService
{
    private readonly AppDbContext _context;

    public CustomSettingService(AppDbContext context)
    {
        _context = context;
    }

    // افزودن تنظیم جدید
    public void AddSetting(string key, string value, string type)
    {
        var setting = new CustomSetting
        {
            Key = key,
            Value = value,
            Type = type
        };
        _context.CustomSettings.Add(setting);
        _context.SaveChanges();
    }

    // دریافت تنظیم بر اساس کلید
    public CustomSetting GetSettingByKey(string key)
    {
        return _context.CustomSettings.FirstOrDefault(s => s.Key == key);
    }

    // به‌روزرسانی تنظیم
    public void UpdateSetting(string key, string newValue)
    {
        var setting = _context.CustomSettings.FirstOrDefault(s => s.Key == key);
        if (setting != null)
        {
            setting.Value = newValue;
            _context.SaveChanges();
        }
    }

    // حذف تنظیم
    public void DeleteSetting(string key)
    {
        var setting = _context.CustomSettings.FirstOrDefault(s => s.Key == key);
        if (setting != null)
        {
            _context.CustomSettings.Remove(setting);
            _context.SaveChanges();
        }
    }

    // دریافت تمام تنظیمات با استفاده از LINQ
    public IEnumerable<CustomSetting> GetAllSettings()
    {
        return _context.CustomSettings.ToList();
    }
}