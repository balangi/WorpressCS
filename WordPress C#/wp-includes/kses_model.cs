using System;
using System.Collections.Generic;

public class AllowedHtmlTag
{
    public string TagName { get; set; }
    public Dictionary<string, bool> Attributes { get; set; } = new Dictionary<string, bool>();
}

public class KsesSettings
{
    public List<AllowedHtmlTag> AllowedTags { get; set; } = new List<AllowedHtmlTag>();
    public List<string> AllowedProtocols { get; set; } = new List<string>();
    public List<string> AllowedEntities { get; set; } = new List<string>();
}