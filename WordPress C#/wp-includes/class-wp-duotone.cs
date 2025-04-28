using System;
using System.Collections.Generic;
using System.Linq;

public class DuotoneManager
{
    private readonly AppDbContext _context;

    public DuotoneManager(AppDbContext context)
    {
        _context = context;
    }

    // افزودن تنظیم Duotone جدید
    public void AddDuotoneSetting(string name, string primaryColor, string secondaryColor, string cssSelector)
    {
        var generatedCss = GenerateCss(cssSelector, primaryColor, secondaryColor);

        var setting = new DuotoneSetting
        {
            Name = name,
            PrimaryColor = primaryColor,
            SecondaryColor = secondaryColor,
            CssSelector = cssSelector,
            GeneratedCss = generatedCss
        };

        _context.DuotoneSettings.Add(setting);
        _context.SaveChanges();
    }

    // دریافت تنظیم Duotone بر اساس شناسه
    public DuotoneSetting GetDuotoneSetting(int id)
    {
        return _context.DuotoneSettings.FirstOrDefault(d => d.Id == id);
    }

    // حذف تنظیم Duotone
    public void RemoveDuotoneSetting(int id)
    {
        var setting = _context.DuotoneSettings.FirstOrDefault(d => d.Id == id);
        if (setting != null)
        {
            _context.DuotoneSettings.Remove(setting);
            _context.SaveChanges();
        }
    }

    // تولید CSS برای افکت Duotone
    private string GenerateCss(string cssSelector, string primaryColor, string secondaryColor)
    {
        return $@"
            {cssSelector} {{
                filter: url('data:image/svg+xml;charset=utf-8,<svg xmlns=\"http://www.w3.org/2000/svg\"><filter id=\"duotone\"><feColorMatrix type=\"matrix\" values=\"0 0 0 0 {primaryColor} 0 0 0 0 {secondaryColor} 0 0 0 0 1 0 0 0 1 0\" /></filter></svg>#duotone');
            }}
        ";
    }

    // دریافت تمام تنظیمات Duotone
    public IEnumerable<DuotoneSetting> GetAllDuotoneSettings()
    {
        return _context.DuotoneSettings.ToList();
    }
}