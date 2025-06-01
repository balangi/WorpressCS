using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

public class AtomCommentsFeedService
{
    private readonly ApplicationDbContext _context;

    public AtomCommentsFeedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public string GenerateAtomCommentsFeed()
    {
        var feed = new XElement("feed",
            new XAttribute("xmlns", "http://www.w3.org/2005/Atom"),
            new XAttribute("xml:lang", GetBlogLanguage()),
            new XAttribute("xmlns:thr", "http://purl.org/syndication/thread/1.0"),
            new XElement("title", new XAttribute("type", "text"), GetFeedTitle()),
            new XElement("subtitle", new XAttribute("type", "text"), GetBlogDescription()),
            new XElement("updated", GetFeedBuildDate()),
            GetFeedLinks(),
            GetFeedEntries()
        );

        return new XDocument(feed).ToString();
    }

    private string GetBlogLanguage()
    {
        return "en-US"; // Replace with actual logic to retrieve blog language
    }

    private string GetFeedTitle()
    {
        if (_context.Posts.Count() == 1)
        {
            var post = _context.Posts.First();
            return $"Comments on {post.Title}";
        }
        else
        {
            return $"Comments for {GetBlogTitle()}";
        }
    }

    private string GetBlogTitle()
    {
        return "Sample Blog Title"; // Replace with actual logic to retrieve blog title
    }

    private string GetBlogDescription()
    {
        return "Sample Blog Description"; // Replace with actual logic to retrieve blog description
    }

    private string GetFeedBuildDate()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    private XElement GetFeedLinks()
    {
        if (_context.Posts.Count() == 1)
        {
            var post = _context.Posts.First();
            return new XElement("links",
                new XElement("link", new XAttribute("rel", "alternate"), new XAttribute("type", "text/html"), new XAttribute("href", GetPostCommentsLink(post.Id))),
                new XElement("link", new XAttribute("rel", "self"), new XAttribute("type", "application/atom+xml"), new XAttribute("href", GetPostCommentsFeedLink(post.Id)))
            );
        }
        else
        {
            return new XElement("links",
                new XElement("link", new XAttribute("rel", "alternate"), new XAttribute("type", "text/html"), new XAttribute("href", GetBlogUrl())),
                new XElement("link", new XAttribute("rel", "self"), new XAttribute("type", "application/atom+xml"), new XAttribute("href", GetCommentsFeedLink()))
            );
        }
    }

    private string GetPostCommentsLink(int postId)
    {
        return $"https://example.com/posts/{postId}/comments";  // Replace with actual logic
    }

    private string GetPostCommentsFeedLink(int postId)
    {
        return $"https://example.com/posts/{postId}/comments.atom";  // Replace with actual logic
    }

    private string GetBlogUrl()
    {
        return "https://example.com";  // Replace with actual logic
    }

    private string GetCommentsFeedLink()
    {
        return "https://example.com/comments.atom";  // Replace with actual logic
    }

    private XElement GetFeedEntries()
    {
        var entries = new XElement("entries");

        foreach (var comment in _context.Comments)
        {
            var entry = new XElement("entry",
                new XElement("title", new XAttribute("type", "text"), GetCommentTitle(comment)),
                new XElement("link", new XAttribute("rel", "alternate"), new XAttribute("type", "text/html"), new XAttribute("href", GetCommentLink(comment.Id))),
                new XElement("author",
                    new XElement("name", comment.Author),
                    !string.IsNullOrEmpty(comment.AuthorUrl) ? new XElement("uri", comment.AuthorUrl) : null
                ),
                new XElement("id", GetCommentGuid(comment.Id)),
                new XElement("updated", comment.Date.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                new XElement("published", comment.Date.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                new XElement("content", new XAttribute("type", "html"), new XAttribute("xml:base", GetCommentLink(comment.Id)), new XCData(comment.Content)),
                GetInReplyToElement(comment)
            );

            entries.Add(entry);
        }

        return entries;
    }

    private string GetCommentTitle(Comment comment)
    {
        if (_context.Posts.Count() == 1)
        {
            return $"By: {comment.Author}";
        }
        else
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == comment.PostId);
            return $"Comment on {post?.Title} by {comment.Author}";
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

    private XElement GetInReplyToElement(Comment comment)
    {
        if (comment.ParentCommentId == 0)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == comment.PostId);
            return new XElement("thr:in-reply-to",
                new XAttribute("ref", GetPostGuid(post.Id)),
                new XAttribute("href", GetPostPermalink(post.Id)),
                new XAttribute("type", "text/html")
            );
        }
        else
        {
            var parentComment = _context.Comments.FirstOrDefault(c => c.Id == comment.ParentCommentId);
            return new XElement("thr:in-reply-to",
                new XAttribute("ref", GetCommentGuid(parentComment.Id)),
                new XAttribute("href", GetCommentLink(parentComment.Id)),
                new XAttribute("type", "text/html")
            );
        }
    }

    private string GetPostGuid(int postId)
    {
        return $"urn:uuid:{postId}";
    }

    private string GetPostPermalink(int postId)
    {
        return $"https://example.com/posts/{postId}";  // Replace with actual logic
    }
}