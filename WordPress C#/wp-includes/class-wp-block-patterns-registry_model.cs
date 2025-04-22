using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class BlockPattern
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public List<string> BlockTypes { get; set; } = new List<string>();
        public List<string> PostTypes { get; set; } = new List<string>();
        public List<string> TemplateTypes { get; set; } = new List<string>();
        public string FilePath { get; set; }
        public bool IsRegisteredOutsideInit { get; set; }
    }
}