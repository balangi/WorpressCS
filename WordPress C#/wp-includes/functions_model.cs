using System;
using System.Collections.Generic;

public class WordPressSettings
{
    public string SiteUrl { get; set; }
    public string Charset { get; set; }
    public string Language { get; set; }
    public int MemoryLimit { get; set; } = 256; // Default memory limit in MB
    public bool UseBalanceTags { get; set; } = true;
}

public class UploadFileResult
{
    public string File { get; set; }
    public string Url { get; set; }
    public string Error { get; set; }
}