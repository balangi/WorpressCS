public class Hook
{
    public int Id { get; set; }
    public string HookName { get; set; } // نام هک
    public string Callback { get; set; } // تابع فراخوانی
    public int Priority { get; set; } // اولویت
}