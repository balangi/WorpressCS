using System.Collections.Generic;

public class ScriptModule
{
    public int Id { get; set; }
    public string ModuleId { get; set; } // Unique identifier for the script module
    public string Src { get; set; } // URL or path of the script module
    public string Version { get; set; } // Version of the script module
    public bool Enqueued { get; set; } // Whether the module is marked for enqueue
    public ICollection<ScriptModuleDependency> Dependencies { get; set; }
}

public class ScriptModuleDependency
{
    public int Id { get; set; }
    public int ScriptModuleId { get; set; }
    public ScriptModule ScriptModule { get; set; }

    public string DependencyId { get; set; } // Identifier of the dependency
    public string ImportType { get; set; } // 'static' or 'dynamic'
}