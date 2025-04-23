using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class BlockTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string Plugin { get; set; }
        public string Content { get; set; }
        public string Source { get; set; }
        public string Slug { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Origin { get; set; }
        public bool IsCustom { get; set; }
        public List<string> PostTypes { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}