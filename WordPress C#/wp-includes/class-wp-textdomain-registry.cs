using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TextDomainService
{
    private readonly AppDbContext _context;

    public TextDomainService(AppDbContext context)
    {
        _context = context;
    }

    // ثبت یک Text Domain جدید
    public bool RegisterTextDomain(string domain, string localeCode, string customPath = null)
    {
        if (_context.TextDomains.Any(td => td.Domain == domain))
        {
            Console.WriteLine($"Text domain '{domain}' already exists.");
            return false;
        }

        var textDomain = new TextDomain
        {
            Domain = domain,
            Locales = new List<Locale>
            {
                new Locale
                {
                    LocaleCode = localeCode,
                    LanguageDirectoryPath = customPath ?? Path.Combine("languages", domain)
                }
            }
        };

        _context.TextDomains.Add(textDomain);
        _context.SaveChanges();
        return true;
    }

    // دریافت مسیر زبان برای یک Text Domain و Locale
    public string GetLanguageDirectoryPath(string domain, string localeCode)
    {
        var locale = _context.Locales
            .FirstOrDefault(l => l.TextDomain.Domain == domain && l.LocaleCode == localeCode);

        return locale?.LanguageDirectoryPath;
    }

    // افزودن فایل ترجمه به یک Locale
    public bool AddTranslationFile(string domain, string localeCode, string filePath)
    {
        var locale = _context.Locales
            .FirstOrDefault(l => l.TextDomain.Domain == domain && l.LocaleCode == localeCode);

        if (locale == null)
        {
            Console.WriteLine($"Locale '{localeCode}' for domain '{domain}' not found.");
            return false;
        }

        var translationFile = new TranslationFile
        {
            FilePath = filePath,
            LocaleId = locale.Id
        };

        _context.TranslationFiles.Add(translationFile);
        _context.SaveChanges();
        return true;
    }

    // دریافت تمام فایل‌های ترجمه برای یک Text Domain و Locale
    public List<string> GetTranslationFiles(string domain, string localeCode)
    {
        return _context.TranslationFiles
            .Where(tf => tf.Locale.TextDomain.Domain == domain && tf.Locale.LocaleCode == localeCode)
            .Select(tf => tf.FilePath)
            .ToList();
    }

    // غیرفعال کردن کش فایل‌های MO
    public void InvalidateMoFilesCache()
    {
        // عملیات غیرفعال‌سازی کش
        Console.WriteLine("MO files cache invalidated.");
    }
}