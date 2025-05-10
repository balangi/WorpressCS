
/// <summary>
/// مدل داده‌ای برای ذخیره خطاها در دیتابیس
/// </summary>
public class PausedExtensionEntity
{
    public int Id { get; set; }
    public string OptionName { get; set; }
    public string Type { get; set; }
    public string Extension { get; set; }
    public int ErrorType { get; set; }
    public string File { get; set; }
    public int Line { get; set; }
    public string Message { get; set; }
}