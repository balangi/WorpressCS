using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Comments
{
    public class WalkerComment
    {
        /// <summary>
        /// Type of the tree structure this walker handles.
        /// </summary>
        public string TreeType { get; set; } = "comment";

        /// <summary>
        /// Database fields to use for the tree structure.
        /// </summary>
        public Dictionary<string, string> DbFields { get; set; } = new Dictionary<string, string>
        {
            { "parent", "ParentId" },
            { "id", "Id" }
        };

        private readonly AppDbContext _dbContext;

        public WalkerComment(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Starts the list before the elements are added.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="depth">Depth of comment.</param>
        /// <param name="args">Optional arguments.</param>
        public void StartLevel(StringBuilder output, int depth = 0, Dictionary<string, object> args = null)
        {
            if (args == null || !args.ContainsKey("style"))
                return;

            var style = args["style"].ToString();
            switch (style)
            {
                case "div":
                    break;
                case "ol":
                    output.AppendLine("<ol class=\"children\">");
                    break;
                case "ul":
                default:
                    output.AppendLine("<ul class=\"children\">");
                    break;
            }
        }

        /// <summary>
        /// Ends the list after the elements are added.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="depth">Depth of comment.</param>
        /// <param name="args">Optional arguments.</param>
        public void EndLevel(StringBuilder output, int depth = 0, Dictionary<string, object> args = null)
        {
            if (args == null || !args.ContainsKey("style"))
                return;

            var style = args["style"].ToString();
            switch (style)
            {
                case "div":
                    break;
                case "ol":
                    output.AppendLine("</ol><!-- .children -->");
                    break;
                case "ul":
                default:
                    output.AppendLine("</ul><!-- .children -->");
                    break;
            }
        }

        /// <summary>
        /// Starts the element output.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="comment">Comment data object.</param>
        /// <param name="depth">Depth of comment.</param>
        /// <param name="args">Optional arguments.</param>
        public void StartElement(StringBuilder output, Comment comment, int depth = 0, Dictionary<string, object> args = null)
        {
            if (comment == null)
                return;

            var tag = args != null && args.ContainsKey("style") && args["style"].ToString() == "div" ? "div" : "li";
            var moderationNote = comment.IsApproved ? "" : "Your comment is awaiting moderation.";

            output.AppendLine($"<{tag} id=\"comment-{comment.Id}\" class=\"comment-body\">");
            output.AppendLine("<div class=\"comment-author vcard\">");
            output.AppendLine($"<cite class=\"fn\">{EscapeHtml(comment.Author)}</cite>");
            output.AppendLine("</div>");

            if (!comment.IsApproved)
            {
                output.AppendLine($"<em class=\"comment-awaiting-moderation\">{moderationNote}</em>");
            }

            output.AppendLine("<div class=\"comment-meta commentmetadata\">");
            output.AppendLine($"<a href=\"#comment-{comment.Id}\">{comment.Date.ToShortDateString()} at {comment.Date.ToShortTimeString()}</a>");
            output.AppendLine("</div>");

            output.AppendLine("<div class=\"comment-content\">");
            output.AppendLine(EscapeHtml(comment.Content));
            output.AppendLine("</div>");
        }

        /// <summary>
        /// Ends the element output.
        /// </summary>
        /// <param name="output">Used to append additional content.</param>
        /// <param name="args">Optional arguments.</param>
        public void EndElement(StringBuilder output, Dictionary<string, object> args = null)
        {
            var tag = args != null && args.ContainsKey("style") && args["style"].ToString() == "div" ? "div" : "li";
            output.AppendLine($"</{tag}>");
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