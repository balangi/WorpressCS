using System.Collections.Generic;

public class Script
{
    public int Id { get; set; }
    public string Handle { get; set; } // Unique identifier for the script
    public string Src { get; set; } // URL or path of the script
    public string Version { get; set; } // Version of the script
    public bool Enqueued { get; set; } // Whether the script is marked for enqueue
    public ICollection<ScriptDependency> Dependencies { get; set; }
}

public class ScriptDependency
{
    public int Id { get; set; }
    public int ScriptId { get; set; }
    public Script Script { get; set; }

    public string DependencyHandle { get; set; } // Identifier of the dependency
    public string Strategy { get; set; } // 'defer', 'async', or null
}