public class EditorSetting
{
    public int Id { get; set; }
    public string EditorId { get; set; } // شناسه ویرایشگر
    public string Content { get; set; } // محتوای اولیه
    public bool UseTinyMCE { get; set; } // آیا از TinyMCE استفاده شود؟
    public bool UseQuickTags { get; set; } // آیا از Quicktags استفاده شود؟
    public string CustomCSS { get; set; } // CSS سفارشی
}