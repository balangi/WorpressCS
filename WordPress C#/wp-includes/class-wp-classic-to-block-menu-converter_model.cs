using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int? ParentId { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public bool OpensInNewTab { get; set; }
        public string Rel { get; set; }
        public string Kind { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}