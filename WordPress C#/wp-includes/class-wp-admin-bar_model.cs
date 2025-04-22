using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WordPress.Core.Models
{
    public class ToolbarNode
    {
        [Key]
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; } // 'container', 'group', 'item'
        public string Title { get; set; }
        public string Href { get; set; }
        public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
        public List<ToolbarNode> Children { get; set; } = new List<ToolbarNode>();
    }
}