using System;
using System.Collections.Generic;

public class Term
{
    public int TermId { get; set; } // شناسه Term
    public string Name { get; set; } // نام Term
    public string Slug { get; set; } // Slug Term
    public int TermGroup { get; set; } // گروه Term
    public List<TermTaxonomy> Taxonomies { get; set; } // ارتباط با TermTaxonomy
}

public class TermTaxonomy
{
    public int TermTaxonomyId { get; set; } // شناسه TermTaxonomy
    public int TermId { get; set; } // Foreign Key به جدول Term
    public string Taxonomy { get; set; } // نام Taxonomy
    public string Description { get; set; } // توضیحات
    public int Parent { get; set; } // والد
    public int Count { get; set; } // تعداد استفاده
    public Term Term { get; set; } // ارتباط با Term
}

public class TermMeta
{
    public int Id { get; set; } // شناسه متا
    public int TermId { get; set; } // Foreign Key به جدول Term
    public string MetaKey { get; set; } // کلید متا
    public string MetaValue { get; set; } // مقدار متا
    public Term Term { get; set; } // ارتباط با Term
}