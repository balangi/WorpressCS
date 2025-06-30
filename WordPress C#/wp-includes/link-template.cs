using System;
using System.Linq;
using System.Text;

public class LinkTemplateService
{
    private readonly ApplicationDbContext _context;

    public LinkTemplateService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays the permalink for the current post.
    /// </summary>
    public string GetPostPermalink(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            throw new ArgumentException("Post not found.");
        }

        return post.Permalink ?? GeneratePermalink(post.Title);
    }

    /// <summary>
    /// Generates a shortlink for the current post.
    /// </summary>
    public string GetShortLink(int postId, string text = "This is the short link.")
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            throw new ArgumentException("Post not found.");
        }

        var shortLink = post.ShortLink ?? GenerateShortLink(post.Id);
        return $"<a href=\"{shortLink}\">{text}</a>";
    }

    /// <summary>
    /// Retrieves the feed link for a category.
    /// </summary>
    public string GetCategoryFeedLink(int categoryId, string feedType = "")
    {
        var category = _context.Terms.FirstOrDefault(t => t.Id == categoryId && t.Taxonomy == "category");
        if (category == null)
        {
            throw new ArgumentException("Category not found.");
        }

        var feed = string.IsNullOrEmpty(feedType) ? "rss2" : feedType;
        return $"{_context.SiteSettings.HomeUrl}/?feed={feed}&cat={categoryId}";
    }

    /// <summary>
    /// Retrieves the edit link for a user.
    /// </summary>
    public string GetUserEditLink(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        return $"{_context.SiteSettings.AdminUrl}/user-edit.php?user_id={userId}";
    }

    /// <summary>
    /// Retrieves the previous comments page link.
    /// </summary>
    public string GetPreviousCommentsLink(int currentPage, string label = "&laquo; Older Comments")
    {
        if (currentPage <= 1)
        {
            return string.Empty;
        }

        var previousPage = currentPage - 1;
        return $"<a href=\"?cpage={previousPage}\">{label}</a>";
    }

    /// <summary>
    /// Generates a permalink based on the title.
    /// </summary>
    private string GeneratePermalink(string title)
    {
        return $"{_context.SiteSettings.HomeUrl}/{title.ToLower().Replace(" ", "-")}";
    }

    /// <summary>
    /// Generates a shortlink based on the post ID.
    /// </summary>
    private string GenerateShortLink(int postId)
    {
        return $"{_context.SiteSettings.HomeUrl}/?p={postId}";
    }
}