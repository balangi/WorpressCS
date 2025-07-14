using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class PostTemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostTemplateService> _logger;

    public PostTemplateService(ApplicationDbContext context, ILogger<PostTemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the excerpt of a post.
    /// </summary>
    public string GetPostExcerpt(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            return null;
        }

        return !string.IsNullOrEmpty(post.Excerpt) ? post.Excerpt : TruncateContent(post.Content);
    }

    /// <summary>
    /// Truncates the content of a post to create an excerpt.
    /// </summary>
    private string TruncateContent(string content, int maxLength = 55)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        return content.Length > maxLength ? content.Substring(0, maxLength) + "..." : content;
    }

    /// <summary>
    /// Displays the classes for the post container element.
    /// </summary>
    public List<string> GetPostClasses(int postId, List<string> additionalClasses = null)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            return new List<string>();
        }

        var classes = new List<string>
        {
            $"{post.Type}-template-default",
            $"single-{post.Type}",
            $"postid-{post.Id}"
        };

        if (post.Type == "post" && post.Status == "publish")
        {
            classes.Add("single");
        }

        if (additionalClasses != null)
        {
            classes.AddRange(additionalClasses);
        }

        return classes;
    }

    /// <summary>
    /// Checks if a post has a custom excerpt.
    /// </summary>
    public bool HasCustomExcerpt(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            return false;
        }

        return !string.IsNullOrEmpty(post.Excerpt);
    }

    /// <summary>
    /// Retrieves the template for a specific post type.
    /// </summary>
    public PostTemplate GetPostTemplate(string templateName)
    {
        return _context.PostTemplates.FirstOrDefault(t => t.TemplateName == templateName);
    }

    /// <summary>
    /// Renders the content of a post using its associated template.
    /// </summary>
    public string RenderPostContent(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            return null;
        }

        var template = GetPostTemplate(post.Type);
        if (template == null)
        {
            return post.Content;
        }

        return template.TemplateContent.Replace("{{content}}", post.Content);
    }
}