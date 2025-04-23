using System;
using System.Collections.Generic;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockTemplates
{
    public class BlockTemplateManager
    {
        /// <summary>
        /// Type: wp_template.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Theme.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Template slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// ID.
        /// </summary>
        public string Id => $"{Theme}//{Slug}";

        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Source of the content.
        /// </summary>
        public string Source { get; set; } = "theme";

        /// <summary>
        /// Origin of the content when customized.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Post ID.
        /// </summary>
        public int? WpId { get; set; }

        /// <summary>
        /// Template Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Whether a template is, or is based upon, an existing template file.
        /// </summary>
        public bool HasThemeFile { get; set; }

        /// <summary>
        /// Whether a template is a custom template.
        /// </summary>
        public bool IsCustom { get; set; } = true;

        /// <summary>
        /// Author.
        /// </summary>
        public int? Author { get; set; }

        /// <summary>
        /// Plugin.
        /// </summary>
        public string Plugin { get; set; }

        /// <summary>
        /// Post types.
        /// </summary>
        public List<string> PostTypes { get; set; } = new List<string>();

        /// <summary>
        /// Area.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Modified.
        /// </summary>
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Saves the block template to the database using EF Core.
        /// </summary>
        public void Save(AppDbContext dbContext)
        {
            var template = new Models.BlockTemplate
            {
                Type = this.Type,
                Theme = this.Theme,
                Slug = this.Slug,
                Title = this.Title,
                Content = this.Content,
                Description = this.Description,
                Source = this.Source,
                Origin = this.Origin,
                WpId = this.WpId,
                Status = this.Status,
                HasThemeFile = this.HasThemeFile,
                IsCustom = this.IsCustom,
                Author = this.Author,
                Plugin = this.Plugin,
                PostTypes = this.PostTypes,
                Area = this.Area,
                Modified = this.Modified
            };

            dbContext.BlockTemplates.Add(template);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Loads a block template from the database using EF Core.
        /// </summary>
        public static BlockTemplateManager Load(int id, AppDbContext dbContext)
        {
            var template = dbContext.BlockTemplates.FirstOrDefault(t => t.Id == id);

            if (template == null)
            {
                throw new InvalidOperationException("Block template not found.");
            }

            return new BlockTemplateManager
            {
                Type = template.Type,
                Theme = template.Theme,
                Slug = template.Slug,
                Title = template.Title,
                Content = template.Content,
                Description = template.Description,
                Source = template.Source,
                Origin = template.Origin,
                WpId = template.WpId,
                Status = template.Status,
                HasThemeFile = template.HasThemeFile,
                IsCustom = template.IsCustom,
                Author = template.Author,
                Plugin = template.Plugin,
                PostTypes = template.PostTypes,
                Area = template.Area,
                Modified = template.Modified
            };
        }
    }
}