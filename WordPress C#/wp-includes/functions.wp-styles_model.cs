using System;
using System.Collections.Generic;

public class Style
{
    public string Handle { get; set; }
    public string Src { get; set; }
    public List<string> Dependencies { get; set; } = new List<string>();
    public string Version { get; set; }
    public string Media { get; set; } = "all";
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    public List<string> InlineStyles { get; set; } = new List<string>();
}

public class StyleRegistry
{
    public Dictionary<string, Style> Styles { get; set; } = new Dictionary<string, Style>();
}