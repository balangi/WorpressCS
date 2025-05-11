using System;
using System.Collections.Generic;

public class Theme
{
    public int Id { get; set; } // شناسه Theme
    public string Name { get; set; } // نام Theme
    public string Stylesheet { get; set; } // نام فایل Stylesheet
    public string Template { get; set; } // نام فایل Template
    public string ThemeRoot { get; set; } // مسیر ریشه Theme
    public bool IsBlockTheme { get; set; } // آیا Block Theme است؟
    public List<ThemeHeader> Headers { get; set; } // ارتباط با ThemeHeader
    public List<ThemeTemplate> Templates { get; set; } // ارتباط با ThemeTemplate
}

public class ThemeHeader
{
    public int Id { get; set; } // شناسه Header
    public int ThemeId { get; set; } // Foreign Key به جدول Theme
    public string Key { get; set; } // کلید Header
    public string Value { get; set; } // مقدار Header
    public Theme Theme { get; set; } // ارتباط با Theme
}

public class ThemeTemplate
{
    public int Id { get; set; } // شناسه Template
    public int ThemeId { get; set; } // Foreign Key به جدول Theme
    public string FilePath { get; set; } // مسیر فایل Template
    public Theme Theme { get; set; } // ارتباط با Theme
}