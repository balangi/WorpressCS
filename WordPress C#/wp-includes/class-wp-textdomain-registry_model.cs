using System;
using System.Collections.Generic;

public class TextDomain
{
    public int Id { get; set; } // شناسه Text Domain
    public string Domain { get; set; } // نام Text Domain
    public List<Locale> Locales { get; set; } // ارتباط با Locale
}

public class Locale
{
    public int Id { get; set; } // شناسه Locale
    public int TextDomainId { get; set; } // Foreign Key به جدول TextDomain
    public string LocaleCode { get; set; } // کد Locale (مانند en_US)
    public string LanguageDirectoryPath { get; set; } // مسیر پوشه زبان
    public TextDomain TextDomain { get; set; } // ارتباط با TextDomain
    public List<TranslationFile> TranslationFiles { get; set; } // ارتباط با TranslationFile
}

public class TranslationFile
{
    public int Id { get; set; } // شناسه فایل ترجمه
    public int LocaleId { get; set; } // Foreign Key به جدول Locale
    public string FilePath { get; set; } // مسیر فایل ترجمه
    public Locale Locale { get; set; } // ارتباط با Locale
}