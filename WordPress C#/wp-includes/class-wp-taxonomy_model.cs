using System;
using System.Collections.Generic;

public class Taxonomy
{
    public int Id { get; set; } // شناسه Taxonomy
    public string Name { get; set; } // نام Taxonomy
    public string Label { get; set; } // برچسب Taxonomy
    public string Description { get; set; } // توضیحات Taxonomy
    public bool IsPublic { get; set; } // آیا عمومی است؟
    public bool IsPubliclyQueryable { get; set; } // آیا قابل پرس‌وجو عمومی است؟
    public bool IsHierarchical { get; set; } // آیا سلسله مراتبی است؟
    public bool ShowInMenu { get; set; } // آیا در منو نمایش داده شود؟
    public bool ShowInNavMenus { get; set; } // آیا در منوهای ناوبری نمایش داده شود؟
    public bool ShowTagCloud { get; set; } // آیا در ابر برچسب‌ها نمایش داده شود؟
    public bool ShowAdminColumn { get; set; } // آیا ستون جداگانه در پنل مدیریت داشته باشد؟
    public string MetaBoxCallback { get; set; } // Callback مربوط به Meta Box
    public string RewriteSlug { get; set; } // Slug برای بازنویسی URL
    public string QueryVar { get; set; } // Query Var برای پرس‌وجو
    public List<TaxonomyMeta> MetaData { get; set; } // داده‌های متا
}

public class TaxonomyMeta
{
    public int Id { get; set; }
    public int TaxonomyId { get; set; } // Foreign Key به جدول Taxonomy
    public string MetaKey { get; set; }
    public string MetaValue { get; set; }

    // رابطه با جدول Taxonomy
    public Taxonomy Taxonomy { get; set; }
}