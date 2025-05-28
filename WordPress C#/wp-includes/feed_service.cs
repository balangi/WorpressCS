using System;
using System.Collections.Generic;
using System.Linq;

public class FeedService
{
    private readonly ApplicationDbContext _context;

    public FeedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public string GetBlogInfoRss(string show)
    {
        var info = StripTags(GetBlogInfo(show));
        return ConvertChars(info);
    }

    public string GetDefaultFeed()
    {
        return "rss2"; // Default feed type
    }

    public string GetPostTitleRss(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        return post?.Title ?? string.Empty;
    }

    public string GetPostContentFeed(int postId, string feedType = null)
    {
        feedType ??= GetDefaultFeed();
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null) return string.Empty;

        var content = post.Content.Replace("]]>", "]]>");
        return content;
    }

    public string GetPostExcerptRss(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        return post?.Excerpt ?? string.Empty;
    }

    public string GetPostPermalinkRss(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        return post?.Permalink ?? string.Empty;
    }

    public string GetCommentAuthorRss(int commentId)
    {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
        return comment?.Author ?? string.Empty;
    }

    public string GetCommentContentRss(int commentId)
    {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
        return comment?.Content ?? string.Empty;
    }

    public string GetCategoriesRss(int postId, string feedType = null)
    {
        feedType ??= GetDefaultFeed();
        var post = _context.Posts.Include(p => p.Categories).FirstOrDefault(p => p.Id == postId);
        if (post == null) return string.Empty;

        var categories = post.Categories.Select(c => SanitizeTermField(c.Name, "category", feedType == "atom" ? "raw" : "rss"));
        var tags = post.Tags.Select(t => SanitizeTermField(t.Name, "post_tag", feedType == "atom" ? "raw" : "rss"));

        var allTerms = categories.Concat(tags).Distinct();
        return string.Join(Environment.NewLine, allTerms.Select(term => $"<category><![CDATA[{term}]]></category>"));
    }

    private string GetBlogInfo(string show)
    {
        // Implement logic to retrieve blog information based on 'show'
        return "Sample Blog Info";
    }

    private string StripTags(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
    }

    private string ConvertChars(string input)
    {
        return input.Replace("&", "&amp;").Replace("<", "<").Replace(">", ">");
    }

    private string SanitizeTermField(string name, string taxonomy, string filter)
    {
        // Implement sanitization logic based on the filter
        return name;
    }
}