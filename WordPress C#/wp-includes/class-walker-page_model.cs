using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WordPress.Core.Models
{
    public class Page
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? ParentId { get; set; }

        // Navigation property for parent-child relationship
        public virtual Page Parent { get; set; }
        public virtual ICollection<Page> Children { get; set; } = new List<Page>();
    }
}