public class Error
{
    public int Id { get; set; }
    public string Code { get; set; } // کد خطا
    public List<string> Messages { get; set; } = new(); // پیام‌های خطا
    public List<object> Data { get; set; } = new(); // داده‌های مرتبط با خطا
}