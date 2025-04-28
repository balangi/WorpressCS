public class CustomSetting
{
    public int Id { get; set; }
    public string Key { get; set; } // کلید تنظیم
    public string Value { get; set; } // مقدار تنظیم
    public string Type { get; set; } // نوع تنظیم (مثلاً "theme_mod", "option")
    public string DefaultValue { get; set; } // مقدار پیش‌فرض
    public bool IsDirty { get; set; } // آیا تنظیم تغییر کرده است؟
}