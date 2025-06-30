using System.ComponentModel.DataAnnotations;

public class Translation
{
    [Key]
    public int Id { get; set; }

    public string TextDomain { get; set; } = string.Empty; // دامنه متنی (مثل "default", "plugin-name")
    public string Locale { get; set; } = string.Empty; // زبان (مثل "en-US", "fa-IR")
    public string OriginalText { get; set; } = string.Empty; // متن اصلی
    public string TranslatedText { get; set; } = string.Empty; // متن ترجمه‌شده
}