using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Pages
{
    public class WalkerPage
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

        public WalkerPage(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Starts the list before the elements are added.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="depth">Depth of page.</param>
        /// <param name="args">Optional arguments.</param>
        public void StartLevel(StringBuilder output, int depth = 0, Dictionary<string, object> args = null)
        {
            var indent = new string('\t', depth);
            output.AppendLine($"{indent}<ul class=\"children\">");
        }

        /// <summary>
        /// Ends the list after the elements are added.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="depth">Depth of page.</param>
        /// <param name="args">Optional arguments.</param>
        public void EndLevel(StringBuilder output, int depth = 0, Dictionary<string, object> args = null)
        {
            var indent = new string('\t', depth);
            output.AppendLine($"{indent}</ul>");
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
            var indent = new string('\t', depth);

            var cssClasses = new List<string> { $"page-item-{page.Id}" };
            if (args != null && args.ContainsKey("pages_with_children") && ((Dictionary<int, bool>)args["pages_with_children"]).ContainsKey(page.Id))
            {
                cssClasses.Add("page_item_has_children");
            }

            if (currentObjectId > 0)
            {
                if (page.Id == currentObjectId)
                {
                    cssClasses.Add("current_page_item");
                }
                else if (IsAncestor(page, currentObjectId))
                {
                    cssClasses.Add("current_page_ancestor");
                }
                else if (page.Id == GetParentId(currentObjectId))
                {
                    cssClasses.Add("current_page_parent");
                }
            }

            var cssClassString = string.Join(" ", cssClasses);
            cssClassString = !string.IsNullOrEmpty(cssClassString) ? $" class=\"{cssClassString}\"" : "";

            var title = string.IsNullOrEmpty(page.Title) ? $"#{page.Id} (no title)" : page.Title;

            var attributes = new Dictionary<string, string>
            {
                { "href", $"/page/{page.Id}" },
                { "aria-current", page.Id == currentObjectId ? "page" : "" }
            };

            var attributeString = BuildAttributes(attributes);

            output.AppendLine($"{indent}<li{cssClassString}><a{attributeString}>{EscapeHtml(title)}</a>");

            if (args != null && args.ContainsKey("show_date") && args["show_date"].ToString() == "true")
            {
                var date = args.ContainsKey("show_date_type") && args["show_date_type"].ToString() == "modified"
                    ? page.ModifiedDate
                    : page.PostDate;

                var dateFormat = args.ContainsKey("date_format") ? args["date_format"].ToString() : "yyyy-MM-dd";
                output.AppendLine($" {date.ToString(dateFormat)}");
            }
        }

        /// <summary>
        /// Ends the element output.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="page">Page data object.</param>
        /// <param name="depth">Depth of page.</param>
        /// <param name="args">Optional arguments.</param>
        public void EndElement(StringBuilder output, Page page, int depth = 0, Dictionary<string, object> args = null)
        {
            var indent = new string('\t', depth);
            output.AppendLine($"{indent}</li>");
        }

        /// <summary>
        /// Checks if a page is an ancestor of another page.
        /// </summary>
        private bool IsAncestor(Page page, int currentObjectId)
        {
            var currentPage = _dbContext.Pages.Find(currentObjectId);
            while (currentPage?.ParentId != null)
            {
                if (currentPage.ParentId == page.Id)
                {
                    return true;
                }
                currentPage = _dbContext.Pages.Find(currentPage.ParentId);
            }
            return false;
        }

        /// <summary>
        /// Gets the parent ID of a page.
        /// </summary>
        private int GetParentId(int pageId)
        {
            var page = _dbContext.Pages.Find(pageId);
            return page?.ParentId ?? 0;
        }

        /// <summary>
        /// Builds a string of HTML attributes from a dictionary.
        /// </summary>
        private string BuildAttributes(Dictionary<string, string> attributes)
        {
            return string.Join(" ", attributes.Where(a => !string.IsNullOrEmpty(a.Value))
                .Select(a => $"{a.Key}=\"{EscapeAttribute(a.Value)}\""));
        }

        /// <summary>
        /// Escapes HTML attributes.
        /// </summary>
        private string EscapeAttribute(string value)
        {
            return Uri.EscapeDataString(value);
        }

        /// <summary>
        /// Escapes HTML content.
        /// </summary>
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