using System;
using System.Collections.Generic;

public class Site
{
    public int BlogId { get; set; } // شناسه سایت
    public string Domain { get; set; } // دامنه سایت
    public string Path { get; set; } // مسیر سایت
    public int NetworkId { get; set; } // شناسه شبکه
    public DateTime Registered { get; set; } // تاریخ ثبت سایت
    public DateTime LastUpdated { get; set; } // آخرین بروزرسانی
    public bool IsPublic { get; set; } // آیا سایت عمومی است؟
    public bool IsArchived { get; set; } // آیا سایت آرشیو شده است؟
    public bool IsMature { get; set; } // آیا سایت برای محتوای بالغ است؟
    public bool IsSpam { get; set; } // آیا سایت اسپم است؟
    public bool IsDeleted { get; set; } // آیا سایت حذف شده است؟
    public int LanguageId { get; set; } // شناسه زبان

    // رابطه با جدول متادیتا
    public ICollection<SiteMeta> MetaData { get; set; }
}

public class SiteMeta
{
    public int Id { get; set; }
    public int SiteId { get; set; } // Foreign Key به جدول Site
    public string MetaKey { get; set; }
    public string MetaValue { get; set; }

    // رابطه با جدول Site
    public Site Site { get; set; }
}