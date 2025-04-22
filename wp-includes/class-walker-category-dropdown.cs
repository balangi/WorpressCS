using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Taxonomy
{
    public class WalkerCategoryDropdown
    {
        /// <summary>
        /// Type of the tree structure this walker handles.
        /// </summary>
        public string TreeType { get; set; } = "category";

        /// <summary>
        /// Database fields to use for the tree structure.
        /// </summary>
        public Dictionary<string, string> DbFields { get; set; } = new Dictionary<string, string>
        {
            { "parent", "ParentId" },
            { "id", "Id" }
        };

        private readonly AppDbContext _dbContext;

        public WalkerCategoryDropdown(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Starts the element output.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="category">Category data object.</param>
        /// <param name="depth">Depth of category.</param>
        /// <param name="args">Optional arguments.</param>
        /// <param name="currentObjectId">ID of the current category.</param>
        public void StartElement(StringBuilder output, Category category, int depth = 0, Dictionary<string, object> args = null, int currentObjectId = 0)
        {
            var pad = new string(' ', depth * 3);

            var catName = category.Name;

            // Determine the value field
            var valueField = args?.ContainsKey("value_field") == true && category.GetType().GetProperty(args["value_field"].ToString()) != null
                ? args["value_field"].ToString()
                : "Id";

            var value = category.GetType().GetProperty(valueField)?.GetValue(category, null)?.ToString() ?? "";

            output.Append($"\t<option class=\"level-{depth}\" value=\"{EscapeAttribute(value)}\"");

            // Check if this category is selected
            if (args?.ContainsKey("selected") == true && args["selected"].ToString() == value)
            {
                output.Append(" selected=\"selected\"");
            }

            output.Append(">");
            output.Append(pad + EscapeHtml(catName));

            // Show count if enabled
            if (args?.ContainsKey("show_count") == true && Convert.ToBoolean(args["show_count"]))
            {
                output.Append($"&nbsp;&nbsp;({category.Count})");
            }

            output.AppendLine("</option>");
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