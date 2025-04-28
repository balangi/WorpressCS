using System;
using System.Collections.Generic;

public class Dependency
{
    // خصوصیات اصلی
    public string Handle { get; set; } // شناسه منحصر به فرد وابستگی
    public string Src { get; set; } // مسیر فایل (URL یا مسیر محلی)
    public List<string> Deps { get; set; } = new(); // لیست وابستگی‌ها
    public string Ver { get; set; } // نسخه وابستگی
    public object Args { get; set; } // آرگومان‌های سفارشی

    // داده‌های اضافی
    public Dictionary<string, object> Extra { get; set; } = new(); // داده‌های اضافی

    // اطلاعات ترجمه
    public string TextDomain { get; set; } // ترجمه textdomain
    public string TranslationsPath { get; set; } // مسیر فایل‌های ترجمه

    // سازنده
    public Dependency(string handle, string src, List<string> deps, string ver, object args)
    {
        Handle = handle;
        Src = src;
        Deps = deps ?? new List<string>();
        Ver = ver;
        Args = args;
    }

    // افزودن داده‌های اضافی
    public bool AddData(string name, object data)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }
        Extra[name] = data;
        return true;
    }

    // تنظیم اطلاعات ترجمه
    public bool SetTranslations(string domain, string path = "")
    {
        if (string.IsNullOrEmpty(domain))
        {
            return false;
        }
        TextDomain = domain;
        TranslationsPath = path;
        return true;
    }
}