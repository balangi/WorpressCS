using System;
using System.Collections.Generic;

public class Taxonomy
{
    public int Id { get; set; } // شناسه Taxonomy
    public string Name { get; set; } // نام Taxonomy
    public string Label { get; set; } // برچسب Taxonomy
    public bool IsHierarchical { get; set; } // آیا سلسله مراتبی است؟
    public List<Term> Terms { get; set; } // ارتباط با Term
}

public class Term
{
    public int Id { get; set; } // شناسه Term
    public string Name { get; set; } // نام Term
    public string Slug { get; set; } // Slug Term
    public int TaxonomyId { get; set; } // Foreign Key به جدول Taxonomy
    public Taxonomy Taxonomy { get; set; } // ارتباط با Taxonomy
    public List<TermRelationship> TermRelationships { get; set; } // ارتباط با TermRelationship
}

public class TermRelationship
{
    public int Id { get; set; } // شناسه رابطه
    public int ObjectId { get; set; } // شناسه شیء مرتبط
    public int TermId { get; set; } // Foreign Key به جدول Term
    public Term Term { get; set; } // ارتباط با Term
}