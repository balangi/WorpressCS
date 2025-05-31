using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

public class AtomFeedService
{
    private readonly ApplicationDbContext _context;

    public AtomFeedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public string GenerateAtomFeed()
    {
        var feed = new XElement("feed",
            new XAttribute("xmlns", "http://www.w3.org/2005/Atom"),
            new XAttribute("xmlns:thr", "http://purl.org/syndication/thread/1.0"),
            new XAttribute("xml:lang", "en-US"),
            new XElement("title", new XAttribute("type", "text"), GetBlogTitle()),
            new XElement("subtitle", new XAttribute("type", "text"), GetBlogDescription()),
            new XElement("updated", GetFeedBuildDate()),
            new XElement("link", new XAttribute("rel", "alternate"), new XAttribute("type", "text/html"), new XAttribute("href", GetBlogUrl())),
            new XElement("id", GetBlogUrl()),
            new XElement("link", new XAttribute("rel", "self"), new XAttribute("type", "application/atom+xml"), new XAttribute("href", GetSelfLink()))
        );

        foreach (var post in _context.Posts)
        {
            var entry = new XElement("entry",
                new XElement("author",
                    new XElement("name", GetPostAuthor(post.Id)),
                    !string.IsNullOrEmpty(GetPostAuthorUrl(post.Id)) ? new XElement("uri", GetPostAuthorUrl(post.Id)) : null
                ),
                new XElement("title", new XAttribute("type", "html"), new XCData(post.Title)),
                new XElement("link", new XAttribute("rel", "alternate"), new XAttribute("type", "text/html"), new XAttribute("href", post.Permalink)),
                new XElement("id", GetPostGuid(post.Id)),
                new XElement("updated", post.ModifiedDate.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                new XElement("published", post.PublishedDate.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                GetPostCategories(post.Id),
                new XElement("summary", new XAttribute("type", "html"), new XCData(post.Excerpt)),
                !GetOptionUseExcerpt() ? new XElement("content", new XAttribute("type", "html"), new XAttribute("xml:base", post.Permalink), new XCData(post.Content)) : null,
                GetPostEnclosures(post.Id),
                GetPostComments(post.Id)
            );

            feed.Add(entry);
        }

        return new XDocument(feed).ToString();
    }

    private string GetBlogTitle()
    {
        return "Sample Blog Title";
    }

    private string GetBlogDescription()
    {
        return "Sample Blog Description";
    }

    private string GetFeedBuildDate()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    private string GetBlogUrl()
    {
        return "https://example.com ";
    }

    private string GetSelfLink()
    {
        return "https://example.com/feed.atom ";
    }

    private string GetPostAuthor(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        return post?.Author ?? "Unknown Author";
    }

    private string GetPostAuthorUrl(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        return post?.AuthorUrl ?? string.Empty;
    }

    private string GetPostGuid(int postId)
    {
        return $"urn:uuid:{postId}";
    }

    private XElement GetPostCategories(int postId)
    {
        var post = _context.Posts.Include(p => p.Categories).FirstOrDefault(p => p.Id == postId);
        if (post == null) return null;

        var categories = post.Categories.Select(c => new XElement("category", new XAttribute("term", c.Name)));
        return new XElement("categories", categories);
    }

    private bool GetOptionUseExcerpt()
    {
        return false; // Replace with actual logic to check the option
    }

    private XElement GetPostEnclosures(int postId)
    {
        return null; // Replace with actual logic to fetch enclosures
    }

    private XElement GetPostComments(int postId)
    {
        var post = _context.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
        if (post == null || post.Comments.Count == 0) return null;

        var commentCount = post.Comments.Count;
        return new XElement("comments",
            new XElement("link", new XAttribute("rel", "replies"), new XAttribute("type", "text/html"), new XAttribute("href", $"{post.Permalink}#comments"), new XAttribute("thr:count", commentCount)),
            new XElement("link", new XAttribute("rel", "replies"), new XAttribute("type", "application/atom+xml"), new XAttribute("href", $"{post.Permalink}/comments.atom"), new XAttribute("thr:count", commentCount)),
            new XElement("thr:total", commentCount)
        );
    }
}