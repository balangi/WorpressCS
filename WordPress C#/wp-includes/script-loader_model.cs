using System;
using System.Collections.Generic;

public class Script
{
    public string Handle { get; set; }
    public string Src { get; set; }
    public List<string> Dependencies { get; set; } = new List<string>();
    public string Version { get; set; }
    public bool IsInFooter { get; set; }
    public Dictionary<string, object> Localizations { get; set; } = new Dictionary<string, object>();
}