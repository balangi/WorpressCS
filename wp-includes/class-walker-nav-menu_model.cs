using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WordPress.Core.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }
        public int? ParentId { get; set; }

        // Navigation property for parent-child relationship
        public virtual MenuItem Parent { get; set; }
        public virtual ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
    }
}