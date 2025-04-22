using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WordPress.Core.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int? ParentId { get; set; }

        // Navigation property for parent-child relationship
        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; } = new List<Category>();

        public int Count { get; set; } // Number of posts in this category
    }
}