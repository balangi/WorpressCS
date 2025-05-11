using System;
using System.Collections.Generic;

public class Style
{
    public int Id { get; set; } // شناسه استایل
    public string Handle { get; set; } // نام منحصر به فرد استایل
    public string Src { get; set; } // آدرس فایل استایل
    public string Version { get; set; } // نسخه استایل
    public string Media { get; set; } // نوع رسانه (Media Type)
    public string ExtraData { get; set; } // داده‌های اضافی (مانند RTL یا Title)
    public List<StyleDependency> Dependencies { get; set; } // وابستگی‌ها
}

public class StyleDependency
{
    public int Id { get; set; }
    public int StyleId { get; set; } // Foreign Key به جدول Style
    public string DependencyHandle { get; set; } // نام وابستگی

    // رابطه با جدول Style
    public Style Style { get; set; }
}