using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class CategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a list of category objects
    /// </summary>
    public async Task<List<Category>> GetCategoriesAsync(string taxonomy = "category")
    {
        var categories = await _context.Categories
            .Where(c => c.Taxonomy == taxonomy)
            .ToListAsync();

        return categories;
    }

    /// <summary>
    /// Retrieves category data given a category ID or category object
    /// </summary>
    public async Task<Category> GetCategoryAsync(int categoryId)
    {
        return await _context.Categories.FindAsync(categoryId);
    }

    /// <summary>
    /// Retrieves a category based on URL containing the category slug
    /// </summary>
    public async Task<Category> GetCategoryByPathAsync(string categoryPath, bool fullMatch = true)
    {
        categoryPath = Uri.UnescapeDataString(categoryPath);
        categoryPath = categoryPath.Replace("%2F", "/").Replace("%20", " ");
        var categoryPaths = "/" + categoryPath.Trim('/');
        var leafPath = SanitizeTitle(Path.GetFileName(categoryPath));
        var pathSegments = categoryPaths.Split('/');

        var fullPath = string.Join("/", pathSegments
            .Select(p => string.IsNullOrEmpty(p) ? "" : SanitizeTitle(p)));

        var categories = await _context.Categories
            .Where(c => c.Slug == leafPath)
            .ToListAsync();

        if (!categories.Any())
            return null;

        foreach (var category in categories)
        {
            var path = "/" + leafPath;
            var currentCategory = category;

            while (currentCategory.Parent != 0 && currentCategory.Parent != currentCategory.TermId)
            {
                currentCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.TermId == currentCategory.Parent);

                if (currentCategory == null)
                    return null;

                path = "/" + currentCategory.Slug + path;
            }

            if (path == fullPath)
            {
                return await GetCategoryAsync(category.TermId);
            }
        }

        if (!fullMatch)
        {
            return await GetCategoryAsync(categories.First().TermId);
        }

        return null;
    }

    /// <summary>
    /// Retrieves a category object by category slug
    /// </summary>
    public async Task<Category> GetCategoryBySlugAsync(string slug)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    /// <summary>
    /// Retrieves the ID of a category from its name
    /// </summary>
    public async Task<int> GetCategoryIdByNameAsync(string name)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == name);

        return category?.TermId ?? 0;
    }

    /// <summary>
    /// Retrieves the name of a category from its ID
    /// </summary>
    public async Task<string> GetCategoryNameByIdAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        return category?.Name ?? string.Empty;
    }

    /// <summary>
    /// Checks if a category is an ancestor of another category
    /// </summary>
    public async Task<bool> IsCategoryAncestorOfAsync(int parentId, int childId)
    {
        var current = await _context.Categories.FindAsync(childId);

        while (current != null && current.Parent != 0)
        {
            if (current.Parent == parentId)
                return true;

            current = await _context.Categories.FindAsync(current.Parent);
        }

        return false;
    }

    private string SanitizeTitle(string title)
    {
        // Implement your sanitization logic here
        return title.ToLower().Replace(" ", "-");
    }
}