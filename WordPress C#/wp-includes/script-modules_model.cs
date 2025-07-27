using System;
using System.Collections.Generic;

public class ScriptModule
{
    public string Id { get; set; }
    public string Src { get; set; }
    public List<Dependency> Dependencies { get; set; } = new List<Dependency>();
    public string Version { get; set; }
    public bool IsEnqueued { get; set; }
}

public class Dependency
{
    public string Id { get; set; }
    public string ImportType { get; set; } // e.g., "static" or "dynamic"
}