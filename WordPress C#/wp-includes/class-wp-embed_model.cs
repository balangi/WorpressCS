public class EmbedSetting
{
    public int Id { get; set; }
    public string Url { get; set; } // URL مورد نظر برای Embed
    public string HtmlContent { get; set; } // محتوای HTML تولید شده
    public bool UseCache { get; set; } // آیا از کش استفاده شود؟
    public DateTime CachedAt { get; set; } // زمان ذخیره‌سازی کش
}