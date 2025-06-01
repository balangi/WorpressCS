using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

public class RdfFeedService
{
    private readonly ApplicationDbContext _context;

    public RdfFeedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public string GenerateRdfFeed()
    {
        var rdf = new XElement("rdf:RDF",
            new XAttribute("xmlns", "http://purl.org/rss/1.0/"),
            new XAttribute("xmlns:rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#"),
            new XAttribute("xmlns:dc", "http://purl.org/dc/elements/1.1/"),
            new XAttribute("xmlns:sy", "http://purl.org/rss/1.0/modules/syndication/"),
            new XAttribute("xmlns:admin", "http://webns.net/mvcb/"),
            new XAttribute("xmlns:content", "http://purl.org/rss/1.0/modules/content/"),
            new XElement("channel",
                new XAttribute("rdf:about", GetBlogUrl()),
                new XElement("title", GetBlogTitle()),
                new XElement("link", GetBlogUrl()),
                new XElement("description", GetBlogDescription()),
                new XElement("dc:date", GetFeedBuildDate()),
                new XElement("sy:updatePeriod", GetUpdatePeriod()),
                new XElement("sy:updateFrequency", GetUpdateFrequency()),
                new XElement("sy:updateBase", "2000-01-01T12:00+00:00"),
                new XElement("items",
                    new XElement("rdf:Seq",
                        _context.Posts.Select(post => new XElement("rdf:li", new XAttribute("rdf:resource", post.Permalink)))
                    )
                )
            ),
            _context.Posts.Select(post => new XElement("item",
                new XAttribute("rdf:about", post.Permalink),
                new XElement("title", post.Title),
                new XElement("link", post.Permalink),
                new XElement("dc:creator", new XCData(post.Author)),
                new XElement("dc:date", post.PublishedDate.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                GetPostCategoriesRdf(post.Id),
                GetOptionUseExcerpt() ? new XElement("description", new XCData(post.Excerpt)) : new XElement("description", new XCData(post.Excerpt)),
                !GetOptionUseExcerpt() ? new XElement("content:encoded", new XCData(post.Content)) : null
            ))
        );

        return new XDocument(rdf).ToString();
    }

    private string GetBlogUrl()
    {
        return "https://example.com";  // Replace with actual logic to retrieve blog URL
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

    private string GetUpdatePeriod()
    {
        return "hourly"; // Replace with actual logic to retrieve update period
    }

    private string GetUpdateFrequency()
    {
        return "1"; // Replace with actual logic to retrieve update frequency
    }

    private XElement GetPostCategoriesRdf(int postId)
    {
        var post = _context.Posts.Include(p => p.Categories).FirstOrDefault(p => p.Id == postId);
        if (post == null) return null;

        var categories = post.Categories.Select(c => new XElement("dc:subject", new XCData(c.Name)));
        return new XElement("categories", categories);
    }

    private bool GetOptionUseExcerpt()
    {
        return false; // Replace with actual logic to check the option
    }
}