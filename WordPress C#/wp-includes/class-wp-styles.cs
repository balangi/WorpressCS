using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class StylesService
{
    private readonly AppDbContext _context;

    public StylesService(AppDbContext context)
    {
        _context = context;
    }

    // ثبت یک استایل جدید
    public bool RegisterStyle(string handle, string src, string version = null, string media = "all", Dictionary<string, string> extraData = null)
    {
        if (_context.Styles.Any(s => s.Handle == handle))
        {
            Console.WriteLine($"Style with handle '{handle}' already exists.");
            return false;
        }

        var style = new Style
        {
            Handle = handle,
            Src = src,
            Version = version ?? "1.0",
            Media = media,
            ExtraData = extraData != null ? string.Join(",", extraData.Select(kv => $"{kv.Key}={kv.Value}")) : null
        };

        _context.Styles.Add(style);
        _context.SaveChanges();
        return true;
    }

    // افزودن وابستگی به یک استایل
    public bool AddDependency(string handle, string dependencyHandle)
    {
        var style = _context.Styles.FirstOrDefault(s => s.Handle == handle);
        if (style == null)
        {
            Console.WriteLine($"Style with handle '{handle}' not found.");
            return false;
        }

        if (_context.StyleDependencies.Any(sd => sd.StyleId == style.Id && sd.DependencyHandle == dependencyHandle))
        {
            Console.WriteLine($"Dependency '{dependencyHandle}' already exists for style '{handle}'.");
            return false;
        }

        var dependency = new StyleDependency
        {
            StyleId = style.Id,
            DependencyHandle = dependencyHandle
        };

        _context.StyleDependencies.Add(dependency);
        _context.SaveChanges();
        return true;
    }

    // دریافت تمام استایل‌ها
    public List<Style> GetAllStyles()
    {
        return _context.Styles
            .Include(s => s.Dependencies)
            .ToList();
    }

    // دریافت استایل‌ها بر اساس معیارهای خاص
    public List<Style> GetStylesByCriteria(Func<Style, bool> predicate)
    {
        return _context.Styles
            .Include(s => s.Dependencies)
            .Where(predicate)
            .ToList();
    }

    // حذف یک استایل
    public bool DeregisterStyle(string handle)
    {
        var style = _context.Styles.FirstOrDefault(s => s.Handle == handle);
        if (style == null)
        {
            Console.WriteLine($"Style with handle '{handle}' not found.");
            return false;
        }

        _context.Styles.Remove(style);
        _context.SaveChanges();
        return true;
    }
}