using System;
using System.Collections.Generic;

public class Script
{
    public string Handle { get; set; }
    public string Src { get; set; }
    public List<string> Dependencies { get; set; } = new List<string>();
    public string Version { get; set; }
    public bool InFooter { get; set; }
    public string Strategy { get; set; }
    public Dictionary<string, object> Localizations { get; set; } = new Dictionary<string, object>();
    public Dictionary<string, string> InlineScripts { get; set; } = new Dictionary<string, string>();
}

public class ScriptRegistry
{
    public Dictionary<string, Script> Scripts { get; set; } = new Dictionary<string, Script>();
}