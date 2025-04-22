using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class FeedService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;

    public FeedService(
        ApplicationDbContext context, 
        IMemoryCache cache,
        HttpClient httpClient)
    {
        _context = context;
        _cache = cache;
        _httpClient = httpClient;
    }

    public async Task<SyndicationFeed> FetchFeedAsync(string url)
    {
        var cacheKey = $"feed_{url}";
        
        if (_cache.TryGetValue(cacheKey, out SyndicationFeed cachedFeed))
        {
            return cachedFeed;
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = XmlReader.Create(stream);
        
        var feed = SyndicationFeed.Load(reader);
        
        // ذخیره در کش به مدت 1 ساعت
        _cache.Set(cacheKey, feed, TimeSpan.FromHours(1));
        
        // ذخیره در دیتابیس با EF Core
        await SaveFeedToDatabaseAsync(url, feed);
        
        return feed;
    }

    private async Task SaveFeedToDatabaseAsync(string url, SyndicationFeed feed)
    {
        var existingFeed = await _context.Feeds
            .FirstOrDefaultAsync(f => f.Url == url);

        if (existingFeed == null)
        {
            existingFeed = new Feed { Url = url };
            _context.Feeds.Add(existingFeed);
        }

        existingFeed.LastUpdated = DateTime.UtcNow;
        existingFeed.Title = feed.Title?.Text;
        existingFeed.Description = feed.Description?.Text;

        await _context.SaveChangesAsync();
    }
}