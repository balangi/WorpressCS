using System;
using System.Collections.Generic;

public class SiteSettings
{
    public string HomeUrl { get; set; }
    public string SiteUrl { get; set; }
    public bool HttpsMigrationRequired { get; set; }
}

public class HttpsMigrationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
}