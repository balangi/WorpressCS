using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WordPress.Core.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public bool IsApproved { get; set; }
        public int? ParentId { get; set; }

        // Navigation property for parent-child relationship
        public virtual Comment Parent { get; set; }
        public virtual ICollection<Comment> Children { get; set; } = new List<Comment>();
    }
}