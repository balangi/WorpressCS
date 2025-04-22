using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class CategoryTemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly IUrlHelper _urlHelper;

    public CategoryTemplateService(ApplicationDbContext context, IUrlHelper urlHelper)
    {
        _context = context;
        _urlHelper = urlHelper;
    }

    /// <summary>
    /// Gets the category link URL
    /// </summary>
    public string GetCategoryLink(int categoryId)
    {
        var category = _context.Categories.Find(categoryId);
        if (category == null) return string.Empty;

        return _urlHelper.Action("Category", "Posts", new { slug = category.Slug });
    }

    /// <summary>
    /// Gets the parent categories hierarchy as a string
    /// </summary>
    public string GetCategoryParents(int categoryId, bool link = false, string separator = "/", bool nicename = false)
    {
        var parents = new List<Category>();
        var current = _context.Categories.Find(categoryId);

        while (current != null && current.Parent != 0)
        {
            current = _context.Categories.Find(current.Parent);
            if (current != null)
            {
                parents.Insert(0, current);
            }
        }

        var result = new StringBuilder();
        foreach (var parent in parents)
        {
            if (result.Length > 0)
            {
                result.Append(separator);
            }

            if (link)
            {
                result.Append($"<a href=\"{GetCategoryLink(parent.TermId)}\">{(nicename ? parent.Slug : parent.Name)}</a>");
            }
            else
            {
                result.Append(nicename ? parent.Slug : parent.Name);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Gets categories assigned to a post
    /// </summary>
    public List<Category> GetPostCategories(int postId)
    {
        var categories = _context.PostCategories
            .Where(pc => pc.PostId == postId)
            .Include(pc => pc.Category)
            .Select(pc => pc.Category)
            .ToList();

        return categories;
    }

    /// <summary>
    /// Gets category name by ID
    /// </summary>
    public string GetCategoryNameById(int categoryId)
    {
        var category = _context.Categories.Find(categoryId);
        return category?.Name ?? string.Empty;
    }

    /// <summary>
    /// Gets the category list HTML for a post
    /// </summary>
    public string GetCategoryList(int postId, string separator = "", string parents = "")
    {
        var categories = GetPostCategories(postId);
        if (!categories.Any())
        {
            return "<span>Uncategorized</span>";
        }

        var relAttribute = "rel=\"category\""; // Simplified for .NET Core

        var result = new StringBuilder();
        if (string.IsNullOrEmpty(separator))
        {
            result.Append("<ul class=\"post-categories\">");
            foreach (var category in categories)
            {
                result.Append("\n\t<li>");
                
                switch (parents?.ToLower())
                {
                    case "multiple":
                        if (category.Parent != 0)
                        {
                            result.Append(GetCategoryParents(category.TermId, true, separator));
                        }
                        result.Append($"<a href=\"{GetCategoryLink(category.TermId)}\" {relAttribute}>{category.Name}</a></li>");
                        break;
                    case "single":
                        result.Append($"<a href=\"{GetCategoryLink(category.TermId)}\" {relAttribute}>");
                        if (category.Parent != 0)
                        {
                            result.Append(GetCategoryParents(category.TermId, false, separator));
                        }
                        result.Append($"{category.Name}</a></li>");
                        break;
                    default:
                        result.Append($"<a href=\"{GetCategoryLink(category.TermId)}\" {relAttribute}>{category.Name}</a></li>");
                        break;
                }
            }
            result.Append("</ul>");
        }
        else
        {
            for (int i = 0; i < categories.Count; i++)
            {
                if (i > 0)
                {
                    result.Append(separator);
                }

                switch (parents?.ToLower())
                {
                    case "multiple":
                        if (categories[i].Parent != 0)
                        {
                            result.Append(GetCategoryParents(categories[i].TermId, true, separator));
                        }
                        result.Append($"<a href=\"{GetCategoryLink(categories[i].TermId)}\" {relAttribute}>{categories[i].Name}</a>");
                        break;
                    case "single":
                        result.Append($"<a href=\"{GetCategoryLink(categories[i].TermId)}\" {relAttribute}>");
                        if (categories[i].Parent != 0)
                        {
                            result.Append(GetCategoryParents(categories[i].TermId, false, separator));
                        }
                        result.Append($"{categories[i].Name}</a>");
                        break;
                    default:
                        result.Append($"<a href=\"{GetCategoryLink(categories[i].TermId)}\" {relAttribute}>{categories[i].Name}</a>");
                        break;
                }
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Checks if a post is in a specific category
    /// </summary>
    public bool IsPostInCategory(int postId, int categoryId)
    {
        return _context.PostCategories
            .Any(pc => pc.PostId == postId && pc.CategoryId == categoryId);
    }

    /// <summary>
    /// Gets category description
    /// </summary>
    public string GetCategoryDescription(int categoryId)
    {
        var category = _context.Categories.Find(categoryId);
        return category?.Description ?? string.Empty;
    }

    /// <summary>
    /// Generates a dropdown list of categories
    /// </summary>
    public string GetCategoriesDropdown(CategoryDropdownOptions options)
    {
        var query = _context.Categories.AsQueryable();

        if (options.HideEmpty)
        {
            query = query.Where(c => c.Count > 0);
        }

        if (options.Exclude != null && options.Exclude.Any())
        {
            query = query.Where(c => !options.Exclude.Contains(c.TermId));
        }

        if (options.ChildOf > 0)
        {
            var childIds = GetChildCategoryIds(options.ChildOf);
            query = query.Where(c => childIds.Contains(c.TermId));
        }

        query = options.OrderBy switch
        {
            "name" => options.Order == "DESC" 
                ? query.OrderByDescending(c => c.Name) 
                : query.OrderBy(c => c.Name),
            _ => options.Order == "DESC" 
                ? query.OrderByDescending(c => c.TermId) 
                : query.OrderBy(c => c.TermId)
        };

        var categories = query.ToList();

        var html = new StringBuilder();
        html.Append($"<select name=\"{options.Name}\" id=\"{options.Id}\" class=\"{options.Class}\"");

        if (options.TabIndex > 0)
        {
            html.Append($" tabindex=\"{options.TabIndex}\"");
        }

        if (options.Required)
        {
            html.Append(" required");
        }

        html.Append(">");

        if (!string.IsNullOrEmpty(options.ShowOptionAll))
        {
            var selected = options.Selected == 0 ? " selected" : "";
            html.Append($"<option value=\"0\"{selected}>{options.ShowOptionAll}</option>");
        }

        if (!string.IsNullOrEmpty(options.ShowOptionNone))
        {
            var selected = options.Selected == options.OptionNoneValue ? " selected" : "";
            html.Append($"<option value=\"{options.OptionNoneValue}\"{selected}>{options.ShowOptionNone}</option>");
        }

        foreach (var category in categories)
        {
            var selected = category.TermId == options.Selected ? " selected" : "";
            var count = options.ShowCount ? $" ({category.Count})" : "";
            html.Append($"<option value=\"{category.TermId}\"{selected}>{category.Name}{count}</option>");
        }

        html.Append("</select>");

        return html.ToString();
    }

    private List<int> GetChildCategoryIds(int parentId)
    {
        var childIds = new List<int>();
        var children = _context.Categories.Where(c => c.Parent == parentId).ToList();

        foreach (var child in children)
        {
            childIds.Add(child.TermId);
            childIds.AddRange(GetChildCategoryIds(child.TermId));
        }

        return childIds;
    }
}

public class CategoryDropdownOptions
{
    public string ShowOptionAll { get; set; }
    public string ShowOptionNone { get; set; }
    public string OrderBy { get; set; } = "id";
    public string Order { get; set; } = "ASC";
    public bool ShowCount { get; set; }
    public bool HideEmpty { get; set; } = true;
    public int ChildOf { get; set; }
    public List<int> Exclude { get; set; }
    public string Name { get; set; } = "cat";
    public string Id { get; set; }
    public string Class { get; set; } = "postform";
    public int Depth { get; set; }
    public int TabIndex { get; set; }
    public int Selected { get; set; }
    public bool Required { get; set; }
    public int OptionNoneValue { get; set; } = -1;
}