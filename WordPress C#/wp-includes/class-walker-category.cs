using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Taxonomy
{
    public class WalkerCategory
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

        public WalkerCategory(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Starts the list before the elements are added.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="depth">Depth of category.</param>
        /// <param name="args">Optional arguments.</param>
        public void StartLevel(StringBuilder output, int depth = 0, Dictionary<string, object> args = null)
        {
            if (args == null || args.ContainsKey("style") && args["style"].ToString() != "list")
                return;

            var indent = new string('\t', depth);
            output.AppendLine($"{indent}<ul class='children'>");
        }

        /// <summary>
        /// Ends the list after the elements are added.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="depth">Depth of category.</param>
        /// <param name="args">Optional arguments.</param>
        public void EndLevel(StringBuilder output, int depth = 0, Dictionary<string, object> args = null)
        {
            if (args == null || args.ContainsKey("style") && args["style"].ToString() != "list")
                return;

            var indent = new string('\t', depth);
            output.AppendLine($"{indent}</ul>");
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
            if (string.IsNullOrEmpty(category.Name))
                return;

            var atts = new Dictionary<string, string>
            {
                { "href", $"/category/{category.Id}" }
            };

            if (args != null && args.ContainsKey("use_desc_for_title") && !string.IsNullOrEmpty(category.Description))
            {
                atts["title"] = category.Description;
            }

            var attributes = string.Join(" ", atts.Where(a => !string.IsNullOrEmpty(a.Value))
                .Select(a => $"{a.Key}=\"{EscapeAttribute(a.Value)}\""));

            var link = $"<a {attributes}>{EscapeHtml(category.Name)}</a>";

            if (args != null && (args.ContainsKey("feed_image") || args.ContainsKey("feed")))
            {
                link += " ";
                link += "<a href=\"/feed/category/" + category.Id + "\">Feed</a>";
            }

            if (args != null && args.ContainsKey("show_count"))
            {
                link += $" ({category.Children.Count})";
            }

            if (args != null && args.ContainsKey("style") && args["style"].ToString() == "list")
            {
                var cssClasses = new List<string> { "cat-item", $"cat-item-{category.Id}" };

                if (args.ContainsKey("current_category") && (int)args["current_category"] == category.Id)
                {
                    cssClasses.Add("current-cat");
                    link = link.Replace("<a", "<a aria-current=\"page\"");
                }

                var cssClassString = string.Join(" ", cssClasses);
                output.AppendLine($"<li class=\"{cssClassString}\">{link}");
            }
            else
            {
                output.AppendLine(link);
            }
        }

        /// <summary>
        /// Ends the element output.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="args">Optional arguments.</param>
        public void EndElement(StringBuilder output, Dictionary<string, object> args = null)
        {
            if (args == null || args.ContainsKey("style") && args["style"].ToString() != "list")
                return;

            output.AppendLine("</li>");
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