using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class BlockType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
        public List<string> UsesContext { get; set; } = new List<string>();
        public List<string> ProvidesContext { get; set; } = new List<string>();
        public List<string> EditorScriptHandles { get; set; } = new List<string>();
        public List<string> ScriptHandles { get; set; } = new List<string>();
        public List<string> ViewScriptHandles { get; set; } = new List<string>();
        public List<string> EditorStyleHandles { get; set; } = new List<string>();
        public List<string> StyleHandles { get; set; } = new List<string>();
        public List<string> ViewStyleHandles { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}