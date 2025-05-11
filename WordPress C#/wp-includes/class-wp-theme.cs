using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ThemeService
{
    private readonly AppDbContext _context;

    public ThemeService(AppDbContext context)
    {
        _context = context;
    }

    // ثبت یک Theme جدید
    public bool RegisterTheme(string themeDir, string themeRoot)
    {
        if (_context.Themes.Any(t => t.Stylesheet == themeDir))
        {
            Console.WriteLine($"Theme with stylesheet '{themeDir}' already exists.");
            return false;
        }

        var theme = new Theme
        {
            Name = themeDir,
            Stylesheet = themeDir,
            ThemeRoot = themeRoot,
            IsBlockTheme = Directory.Exists(Path.Combine(themeRoot, themeDir, "block-templates")),
            Headers = LoadHeaders(themeRoot, themeDir),
            Templates = LoadTemplates(themeRoot, themeDir)
        };

        _context.Themes.Add(theme);
        _context.SaveChanges();
        return true;
    }

    // بارگذاری Headers از فایل style.css
    private List<ThemeHeader> LoadHeaders(string themeRoot, string themeDir)
    {
        var headers = new List<ThemeHeader>();
        var styleFilePath = Path.Combine(themeRoot, themeDir, "style.css");

        if (File.Exists(styleFilePath))
        {
            var lines = File.ReadAllLines(styleFilePath);
            foreach (var line in lines)
            {
                if (line.Contains(":"))
                {
                    var parts = line.Split(":", 2);
                    headers.Add(new ThemeHeader
                    {
                        Key = parts[0].Trim(),
                        Value = parts[1].Trim()
                    });
                }
            }
        }

        return headers;
    }

    // بارگذاری Templates از پوشه Templates
    private List<ThemeTemplate> LoadTemplates(string themeRoot, string themeDir)
    {
        var templates = new List<ThemeTemplate>();
        var templateDir = Path.Combine(themeRoot, themeDir, "templates");

        if (Directory.Exists(templateDir))
        {
            var files = Directory.GetFiles(templateDir, "*.php");
            foreach (var file in files)
            {
                templates.Add(new ThemeTemplate
                {
                    FilePath = file
                });
            }
        }

        return templates;
    }

    // دریافت یک Theme بر اساس نام
    public Theme GetThemeByName(string name)
    {
        return _context.Themes
            .Include(t => t.Headers)
            .Include(t => t.Templates)
            .FirstOrDefault(t => t.Name == name);
    }

    // حذف یک Theme
    public bool DeleteTheme(string name)
    {
        var theme = _context.Themes.FirstOrDefault(t => t.Name == name);
        if (theme == null)
        {
            Console.WriteLine($"Theme with name '{name}' not found.");
            return false;
        }

        _context.Themes.Remove(theme);
        _context.SaveChanges();
        return true;
    }
}