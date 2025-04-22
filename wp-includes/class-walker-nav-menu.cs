using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Menus
{
    public class WalkerNavMenu
    {
        /// <summary>
        /// Type of the tree structure this walker handles.
        /// </summary>
        public List<string> TreeType { get; set; } = new List<string> { "post_type", "taxonomy", "custom" };

        /// <summary>
        /// Database fields to use for the tree structure.
        /// </summary>
        public Dictionary<string, string> DbFields { get; set; } = new Dictionary<string, string>
        {
            { "parent", "ParentId" },
            { "id", "Id" }
        };

        private readonly AppDbContext _dbContext;

        public WalkerNavMenu(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Starts the list before the elements are added.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="depth">Depth of menu item.</param>
        /// <param name="args">Optional arguments.</param>
        public void StartLevel(StringBuilder output, int depth = 0, Dictionary<string, object> args = null)
        {
            var indent = new string('\t', depth);
            output.AppendLine($"{indent}<ul class=\"sub-menu\">");
        }

        /// <summary>
        /// Ends the list after the elements are added.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="depth">Depth of menu item.</param>
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
        /// <param name="menuItem">Menu item data object.</param>
        /// <param name="depth">Depth of menu item.</param>
        /// <param name="args">Optional arguments.</param>
        public void StartElement(StringBuilder output, MenuItem menuItem, int depth = 0, Dictionary<string, object> args = null)
        {
            var indent = new string('\t', depth);

            var liAttributes = new Dictionary<string, string>
            {
                { "class", $"menu-item-{menuItem.Id}" }
            };

            var liAttributeString = BuildAttributes(liAttributes);

            output.AppendLine($"{indent}<li{liAttributeString}>");

            var aAttributes = new Dictionary<string, string>
            {
                { "href", menuItem.Url },
                { "title", menuItem.Title }
            };

            var aAttributeString = BuildAttributes(aAttributes);

            output.AppendLine($"{indent}\t<a{aAttributeString}>{EscapeHtml(menuItem.Title)}</a>");
        }

        /// <summary>
        /// Ends the element output.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="menuItem">Menu item data object.</param>
        /// <param name="depth">Depth of menu item.</param>
        /// <param name="args">Optional arguments.</param>
        public void EndElement(StringBuilder output, MenuItem menuItem, int depth = 0, Dictionary<string, object> args = null)
        {
            var indent = new string('\t', depth);
            output.AppendLine($"{indent}</li>");
        }

        /// <summary>
        /// Builds a string of HTML attributes from a dictionary.
        /// </summary>
        /// <param name="attributes">The attributes dictionary.</param>
        /// <returns>A string of HTML attributes.</returns>
        private string BuildAttributes(Dictionary<string, string> attributes)
        {
            return string.Join(" ", attributes.Where(a => !string.IsNullOrEmpty(a.Value))
                .Select(a => $"{a.Key}=\"{EscapeAttribute(a.Value)}\""));
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