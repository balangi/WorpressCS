using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Pages
{
    public class WalkerPageDropdown
    {
        /// <summary>
        /// Type of the tree structure this walker handles.
        /// </summary>
        public string TreeType { get; set; } = "page";

        /// <summary>
        /// Database fields to use for the tree structure.
        /// </summary>
        public Dictionary<string, string> DbFields { get; set; } = new Dictionary<string, string>
        {
            { "parent", "ParentId" },
            { "id", "Id" }
        };

        private readonly AppDbContext _dbContext;

        public WalkerPageDropdown(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Starts the element output.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="page">Page data object.</param>
        /// <param name="depth">Depth of page.</param>
        /// <param name="args">Optional arguments.</param>
        /// <param name="currentObjectId">ID of the current page.</param>
        public void StartElement(StringBuilder output, Page page, int depth = 0, Dictionary<string, object> args = null, int currentObjectId = 0)
        {
            var pad = new string(' ', depth * 3);

            // Determine the value field
            var valueField = args?.ContainsKey("value_field") == true && page.GetType().GetProperty(args["value_field"].ToString()) != null
                ? args["value_field"].ToString()
                : "Id";

            var value = page.GetType().GetProperty(valueField)?.GetValue(page, null)?.ToString() ?? "";

            output.Append($"\t<option class=\"level-{depth}\" value=\"{EscapeAttribute(value)}\"");

            // Check if this page is selected
            if (args?.ContainsKey("selected") == true && args["selected"].ToString() == value)
            {
                output.Append(" selected=\"selected\"");
            }

            output.Append(">");

            var title = string.IsNullOrEmpty(page.Title) ? $"#{page.Id} (no title)" : page.Title;

            // Apply filters (if needed)
            title = FilterPageTitle(title, page);

            output.AppendLine(pad + EscapeHtml(title) + "</option>");
        }

        /// <summary>
        /// Filters the page title.
        /// </summary>
        /// <param name="title">The page title.</param>
        /// <param name="page">The page data object.</param>
        /// <returns>The filtered page title.</returns>
        private string FilterPageTitle(string title, Page page)
        {
            // Simulate a filter like WordPress's 'list_pages' filter
            return title; // Placeholder for actual filtering logic
        }

        /// <summary>
        /// Escapes HTML attributes.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        private string EscapeAttribute(string value)
        {
            return Uri.EscapeDataString(value);
        }

        /// <summary>
        /// Escapes HTML content.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        private string EscapeHtml(string value)
        {
            return value.Replace("&", "&amp;")
                        .Replace("<", "<")
                        .Replace(">", ">")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&#39;");
        }
    }
}