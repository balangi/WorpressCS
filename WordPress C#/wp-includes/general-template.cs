using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GeneralTemplateService
{
    private readonly ApplicationDbContext _context;

    public GeneralTemplateService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Loads a template part into the current template.
    /// </summary>
    public string LoadTemplatePart(string slug, string name = null, Dictionary<string, object> args = null)
    {
        var templates = new List<string>();
        if (!string.IsNullOrEmpty(name))
        {
            templates.Add($"{slug}-{name}.cshtml");
        }
        templates.Add($"{slug}.cshtml");

        foreach (var template in templates)
        {
            var templatePath = LocateTemplate(template);
            if (!string.IsNullOrEmpty(templatePath))
            {
                return RenderTemplate(templatePath, args);
            }
        }

        return string.Empty;
    }

    private string LocateTemplate(string templateName)
    {
        // Logic to locate the template file in the file system or database
        return _context.Templates.FirstOrDefault(t => t.Path.EndsWith(templateName))?.Path;
    }

    private string RenderTemplate(string templatePath, Dictionary<string, object> args)
    {
        // Logic to render the template with arguments
        return $"Rendered: {templatePath} with args: {string.Join(", ", args?.Keys ?? new List<string>())}";
    }

    /// <summary>
    /// Generates pagination links.
    /// </summary>
    public string GeneratePaginationLinks(PaginationOptions options)
    {
        var pageLinks = new StringBuilder();

        for (int i = 1; i <= options.TotalPages; i++)
        {
            var link = options.BaseUrl.Replace("%_%", options.Format.Replace("%#%", i.ToString()));
            pageLinks.Append($"<a href='{link}'>{i}</a>");
        }

        if (options.PrevNext)
        {
            if (options.CurrentPage > 1)
            {
                var prevLink = options.BaseUrl.Replace("%_%", options.Format.Replace("%#%", (options.CurrentPage - 1).ToString()));
                pageLinks.Insert(0, $"<a href='{prevLink}'>{options.PrevText}</a>");
            }

            if (options.CurrentPage < options.TotalPages)
            {
                var nextLink = options.BaseUrl.Replace("%_%", options.Format.Replace("%#%", (options.CurrentPage + 1).ToString()));
                pageLinks.Append($"<a href='{nextLink}'>{options.NextText}</a>");
            }
        }

        return pageLinks.ToString();
    }

    /// <summary>
    /// Retrieves feed links for the site.
    /// </summary>
    public List<FeedLink> GetFeedLinks()
    {
        return new List<FeedLink>
        {
            new FeedLink { Type = "rss2", Url = "/feed" },
            new FeedLink { Type = "atom", Url = "/feed/atom" },
            new FeedLink { Type = "comments_rss2", Url = "/comments/feed" }
        };
    }
}