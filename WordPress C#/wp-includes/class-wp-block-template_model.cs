using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class BlockTemplate
    {
        public int Id { get; set; }

        // Type: wp_template
        public string Type { get; set; }

        // Theme
        public string Theme { get; set; }

        // Template slug
        public string Slug { get; set; }

        // Title
        public string Title { get; set; } = string.Empty;

        // Content
        public string Content { get; set; } = string.Empty;

        // Description
        public string Description { get; set; } = string.Empty;

        // Source of the content
        public string Source { get; set; } = "theme";

        // Origin of the content when customized
        public string Origin { get; set; }

        // Post ID
        public int? WpId { get; set; }

        // Template Status
        public string Status { get; set; }

        // Whether a template is, or is based upon, an existing template file
        public bool HasThemeFile { get; set; }

        // Whether a template is a custom template
        public bool IsCustom { get; set; } = true;

        // Author
        public int? Author { get; set; }

        // Plugin
        public string Plugin { get; set; }

        // Post types
        public List<string> PostTypes { get; set; } = new List<string>();

        // Area
        public string Area { get; set; }

        // Modified
        public DateTime? Modified { get; set; }
    }
}