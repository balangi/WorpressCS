using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

public class Rss2FeedService
{
    private readonly ApplicationDbContext _context;

    public Rss2FeedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public string GenerateRss2Feed()
    {
        var rss = new XElement("rss",
            new XAttribute("version", "2.0"),
            new XAttribute("xmlns:content", "http://purl.org/rss/1.0/modules/content/"),
            new XAttribute("xmlns:wfw", "http://wellformedweb.org/CommentAPI/"),
            new XAttribute("xmlns:dc", "http://purl.org/dc/elements/1.1/"),
            new XAttribute("xmlns:atom", "http://www.w3.org/2005/Atom"),
            new XAttribute("xmlns:sy", "http://purl.org/rss/1.0/modules/syndication/"),
            new XAttribute("xmlns:slash", "http://purl.org/rss/1.0/modules/slash/"),
            new XElement("channel",
                new XElement("title", GetBlogTitle()),
                new XElement("atom:link", 
                    new XAttribute("href", GetSelfLink()), 
                    new XAttribute("rel", "self"), 
                    new XAttribute("type", "application/rss+xml")),
                new XElement("link", GetBlogUrl()),
                new XElement("description", GetBlogDescription()),
                new XElement("lastBuildDate", GetFeedBuildDate()),
                new XElement("language", GetBlogLanguage()),
                new XElement("sy:updatePeriod", GetUpdatePeriod()),
                new XElement("sy:updateFrequency", GetUpdateFrequency()),
                GetFeedItems()
            )
        );

        return new XDocument(rss).ToString();
    }

    private string GetBlogTitle()
    {
        return "Sample Blog Title"; // Replace with actual logic to retrieve blog title
    }

    private string GetBlogUrl()
    {
        return "https://example.com";  // Replace with actual logic to retrieve blog URL
    }

    private string GetBlogDescription()
    {
        return "Sample Blog Description"; // Replace with actual logic to retrieve blog description
    }

    private string GetFeedBuildDate()
    {
        return DateTime.UtcNow.ToString("r");
    }

    private string GetBlogLanguage()
    {
        return "en-US"; // Replace with actual logic to retrieve blog language
    }

    private string GetUpdatePeriod()
    {
        return "hourly"; // Replace with actual logic to retrieve update period
    }

    private string GetUpdateFrequency()
    {
        return "1"; // Replace with actual logic to retrieve update frequency
    }

    private string GetSelfLink()
    {
        return "https://example.com/feed.rss";  // Replace with actual logic to retrieve self link
    }

    private XElement GetFeedItems()
    {
        var items = new XElement("items");

        foreach (var post in _context.Posts)
        {
            var item = new XElement("item",
                new XElement("title", post.Title),
                new XElement("link", post.Permalink),
                GetCommentsElement(post),
                new XElement("dc:creator", new XCData(post.Author)),
                new XElement("pubDate", post.PublishedDate.ToString("r")),
                GetCategoriesRss(post.Id),
                new XElement("guid", new XAttribute("isPermaLink", "false"), GetPostGuid(post.Id)),
                GetDescriptionAndContent(post),
                GetCommentRss(post),
                GetEnclosures(post)
            );

            items.Add(item);
        }

        return items;
    }

    private XElement GetCommentsElement(Post post)
    {
        if (post.Comments.Count > 0 || true /* comments_open */)
        {
            return new XElement("comments", GetCommentsLink(post.Id));
        }
        return null;
    }

    private string GetCommentsLink(int postId)
    {
        return $"https://example.com/posts/{postId}/comments";  // Replace with actual logic
    }

    private XElement GetCategoriesRss(int postId)
    {
        var post = _context.Posts.Include(p => p.Categories).FirstOrDefault(p => p.Id == postId);
        if (post == null) return null;

        var categories = post.Categories.Select(c => new XElement("category", new XCData(c.Name)));
        return new XElement("categories", categories);
    }

    private string GetPostGuid(int postId)
    {
        return $"urn:uuid:{postId}";
    }

    private XElement GetDescriptionAndContent(Post post)
    {
        if (GetOptionUseExcerpt())
        {
            return new XElement("description", new XCData(post.Excerpt));
        }
        else
        {
            var content = !string.IsNullOrEmpty(post.Content) ? post.Content : post.Excerpt;
            return new XElement("content:encoded", new XCData(content));
        }
    }

    private bool GetOptionUseExcerpt()
    {
        return false; // Replace with actual logic to check the option
    }

    private XElement GetCommentRss(Post post)
    {
        if (post.Comments.Count > 0 || true /* comments_open */)
        {
            return new XElement("wfw:commentRss", GetPostCommentsFeedLink(post.Id));
        }
        return null;
    }

    private string GetPostCommentsFeedLink(int postId)
    {
        return $"https://example.com/posts/{postId}/comments.rss";  // Replace with actual logic
    }

    private XElement GetEnclosures(Post post)
    {
        // Implement logic to fetch enclosures if needed
        return null;
    }
}