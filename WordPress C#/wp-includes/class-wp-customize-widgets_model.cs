public class Widget
{
    public int Id { get; set; }
    public string Name { get; set; } // نام ویجت
    public string Type { get; set; } // نوع ویجت (مثلاً "text", "image")
    public string SidebarId { get; set; } // شناسه سایدباری که ویجت در آن قرار دارد
    public ICollection<WidgetSetting> Settings { get; set; } // تنظیمات ویجت
}

public class WidgetSetting
{
    public int Id { get; set; }
    public string Key { get; set; } // کلید تنظیم
    public string Value { get; set; } // مقدار تنظیم
    public int WidgetId { get; set; }
    public Widget Widget { get; set; }
}