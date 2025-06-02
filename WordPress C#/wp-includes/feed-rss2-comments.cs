using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

public class Rss2CommentsFeedService
{
    private readonly ApplicationDbContext _context;

    public Rss2CommentsFeedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public string GenerateRss2CommentsFeed()
    {
        var rss = new XElement("rss",
            new XAttribute("version", "2.0"),
            new XAttribute("xmlns:content", "http://purl.org/rss/1.0/modules/content/"),
            new XAttribute("xmlns:dc", "http://purl.org/dc/elements/1.1/"),
            new XAttribute("xmlns:atom", "http://www.w3.org/2005/Atom"),
            new XAttribute("xmlns:sy", "http://purl.org/rss/1.0/modules/syndication/"),
            new XElement("channel",
                new XElement("title", GetFeedTitle()),
                new XElement("atom:link",
                    new XAttribute("href", GetSelfLink()),
                    new XAttribute("rel", "self"),
                    new XAttribute("type", "application/rss+xml")
                ),
                new XElement("link", IsSinglePost() ? GetPostPermalink() : GetBlogUrl()),
                new XElement("description", GetBlogDescription()),
                new XElement("lastBuildDate", GetFeedBuildDate()),
                new XElement("sy:updatePeriod", GetUpdatePeriod()),
                new XElement("sy:updateFrequency", GetUpdateFrequency()),
                GetFeedItems()
            )
        );

        return new XDocument(rss).ToString();
    }

    private string GetFeedTitle()
    {
        if (_context.Posts.Count() == 1)
        {
            var post = _context.Posts.First();
            return $"Comments on: {post.Title}";
        }
        else
        {
            return $"Comments for {GetBlogTitle()}";
        }
    }

    private string GetBlogTitle()
    {
        return "Sample Blog Title"; // Replace with actual logic
    }

    private string GetBlogDescription()
    {
        return "Sample Blog Description"; // Replace with actual logic
    }

    private string GetFeedBuildDate()
    {
        return DateTime.UtcNow.ToString("r");
    }

    private string GetUpdatePeriod()
    {
        return "hourly"; // Replace with actual logic
    }

    private string GetUpdateFrequency()
    {
        return "1"; // Replace with actual logic
    }

    private string GetSelfLink()
    {
        return "https://example.com/comments.rss";  // Replace with actual logic
    }

    private bool IsSinglePost()
    {
        return _context.Posts.Count() == 1;
    }

    private string GetPostPermalink()
    {
        var post = _context.Posts.First();
        return post.Permalink; // Replace with actual logic
    }

    private string GetBlogUrl()
    {
        return "https://example.com";  // Replace with actual logic
    }

    private XElement GetFeedItems()
    {
        var items = new XElement("items");

        foreach (var comment in _context.Comments)
        {
            var item = new XElement("item",
                new XElement("title", GetCommentTitle(comment)),
                new XElement("link", GetCommentLink(comment.Id)),
                new XElement("dc:creator", new XCData(comment.Author)),
                new XElement("pubDate", comment.Date.ToString("r")),
                new XElement("guid", new XAttribute("isPermaLink", "false"), GetCommentGuid(comment.Id)),
                GetCommentContent(comment)
            );

            items.Add(item);
        }

        return items;
    }

    private string GetCommentTitle(Comment comment)
    {
        if (_context.Posts.Count() > 1)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == comment.PostId);
            return $"Comment on {post?.Title} by {comment.Author}";
        }
        else
        {
            return $"By: {comment.Author}";
        }
    }

    private string GetCommentLink(int commentId)
    {
        return $"https://example.com/comments/{commentId}";  // Replace with actual logic
    }

    private string GetCommentGuid(int commentId)
    {
        return $"urn:uuid:{commentId}";
    }

    private XElement GetCommentContent(Comment comment)
    {
        if (IsPostPasswordProtected(comment.PostId))
        {
            return new XElement("description", "Protected Comments: Please enter your password to view comments.");
        }
        else
        {
            return new XElement("description", new XCData(comment.Content));
        }
    }

    private bool IsPostPasswordProtected(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        return false; // Replace with actual logic to check if the post is password-protected
    }
}