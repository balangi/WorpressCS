using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RssService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<RssService> _logger;

    public RssService(IMemoryCache cache, ILogger<RssService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Fetches and parses an RSS feed from a given URL.
    /// </summary>
    public RssFeed FetchRssFeed(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException("URL cannot be null or empty.");
        }

        // Check cache first
        var cacheKey = $"rss_{url}";
        if (_cache.TryGetValue(cacheKey, out RssFeed cachedFeed))
        {
            return cachedFeed;
        }

        try
        {
            using var client = new HttpClient();
            var response = client.GetStringAsync(url).Result;

            var rssFeed = ParseRssFeed(response);
            _cache.Set(cacheKey, rssFeed, TimeSpan.FromMinutes(30)); // Cache for 30 minutes
            return rssFeed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching or parsing RSS feed.");
            throw new InvalidOperationException("Failed to fetch or parse RSS feed.", ex);
        }
    }

    /// <summary>
    /// Parses an RSS feed from XML content.
    /// </summary>
    private RssFeed ParseRssFeed(string xmlContent)
    {
        var rssFeed = new RssFeed();
        var doc = XDocument.Parse(xmlContent);

        var channel = doc.Descendants("channel").FirstOrDefault();
        if (channel == null)
        {
            throw new InvalidOperationException("Invalid RSS feed structure.");
        }

        rssFeed.Title = channel.Element("title")?.Value;
        rssFeed.Link = channel.Element("link")?.Value;
        rssFeed.Description = channel.Element("description")?.Value;

        foreach (var item in channel.Elements("item"))
        {
            rssFeed.Items.Add(new RssItem
            {
                Title = item.Element("title")?.Value,
                Link = item.Element("link")?.Value,
                Description = item.Element("description")?.Value,
                PublishDate = ParseDateTime(item.Element("pubDate")?.Value)
            });
        }

        return rssFeed;
    }

    /// <summary>
    /// Parses a date string into a DateTime object.
    /// </summary>
    private DateTime ParseDateTime(string dateStr)
    {
        if (DateTime.TryParse(dateStr, out var result))
        {
            return result;
        }

        return DateTime.MinValue;
    }

    /// <summary>
    /// Displays RSS items as an HTML list.
    /// </summary>
    public string DisplayRssItems(RssFeed feed, int? maxItems = null)
    {
        var items = maxItems.HasValue ? feed.Items.Take(maxItems.Value) : feed.Items;

        var html = "<ul>";
        foreach (var item in items)
        {
            html += $"<li><a href=\"{item.Link}\" title=\"{item.Description}\">{item.Title}</a></li>";
        }
        html += "</ul>";

        return html;
    }
}