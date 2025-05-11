using System;

public class Term
{
    public int TermId { get; set; } // شناسه Term
    public string Name { get; set; } // نام Term
    public string Slug { get; set; } // Slug Term
    public int TermGroup { get; set; } // گروه Term
    public int TermTaxonomyId { get; set; } // شناسه Taxonomy مرتبط
    public string Taxonomy { get; set; } // نام Taxonomy
    public string Description { get; set; } // توضیحات Term
    public int Parent { get; set; } // والد Term
    public int Count { get; set; } // تعداد استفاده از Term
    public string Filter { get; set; } = "raw"; // سطح Sanitization

    // ارتباط با جدول TermTaxonomy
    public TermTaxonomy TermTaxonomy { get; set; }
}

public class TermTaxonomy
{
    public int TermTaxonomyId { get; set; } // شناسه TermTaxonomy
    public int TermId { get; set; } // Foreign Key به جدول Term
    public string Taxonomy { get; set; } // نام Taxonomy
    public string Description { get; set; } // توضیحات
    public int Parent { get; set; } // والد
    public int Count { get; set; } // تعداد استفاده

    // ارتباط با جدول Term
    public Term Term { get; set; }
}