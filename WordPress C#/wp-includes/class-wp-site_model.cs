using System;
using System.ComponentModel.DataAnnotations;

public class Site
{
    [Key]
    public int BlogId { get; set; } // شناسه سایت

    public string Domain { get; set; } // دامنه سایت
    public string Path { get; set; } // مسیر سایت
    public int NetworkId { get; set; } // شناسه شبکه (Network ID)
    public DateTime Registered { get; set; } // تاریخ ثبت سایت
    public DateTime LastUpdated { get; set; } // آخرین بروزرسانی
    public bool IsPublic { get; set; } // آیا سایت عمومی است؟
    public bool IsArchived { get; set; } // آیا سایت آرشیو شده است؟
    public bool IsMature { get; set; } // آیا سایت برای محتوای بالغ است؟
    public bool IsSpam { get; set; } // آیا سایت اسپم است؟
    public bool IsDeleted { get; set; } // آیا سایت حذف شده است؟
    public int LanguageId { get; set; } // شناسه زبان

    // اطلاعات اضافی (Lazy Loaded)
    public virtual SiteDetails Details { get; set; }
}

public class SiteDetails
{
    [Key]
    public int BlogId { get; set; } // شناسه سایت (Foreign Key)

    public string BlogName { get; set; } // نام سایت
    public string SiteUrl { get; set; } // URL سایت
    public int PostCount { get; set; } // تعداد پست‌ها
    public string Home { get; set; } // صفحه اصلی سایت

    // رابطه با جدول اصلی
    public virtual Site Site { get; set; }
}