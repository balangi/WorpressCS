using System;
using Microsoft.EntityFrameworkCore;

namespace WordPress.Core.Models
{
    public class BlockEditorContext
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PostId { get; set; } // Optional: Foreign key to a Post entity
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}