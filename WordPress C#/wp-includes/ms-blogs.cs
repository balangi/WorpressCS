using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class MsBlogsService
{
    private readonly ApplicationDbContext _context;

    public MsBlogsService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Updates the last_updated field for the current site.
    /// </summary>
    public void UpdateSiteDate(int blogId)
    {
        var site = _context.Sites.FirstOrDefault(s => s.BlogId == blogId);
        if (site != null)
        {
            site.LastUpdated = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    /// <summary>
    /// Gets the full URL of a site by its ID.
    /// </summary>
    public string GetSiteUrlById(int blogId)
    {
        var site = _context.Sites.FirstOrDefault(s => s.BlogId == blogId);
        if (site == null)
        {
            return string.Empty;
        }

        var scheme = Uri.UriSchemeHttp;
        return $"{scheme}://{site.Domain}{site.Path}";
    }

    /// <summary>
    /// Gets the full URL of a site by its name.
    /// </summary>
    public string GetSiteUrlByName(string domain, string path)
    {
        var site = _context.Sites.FirstOrDefault(s =>
            s.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase) &&
            s.Path.Equals(path, StringComparison.OrdinalIgnoreCase));

        if (site == null)
        {
            return string.Empty;
        }

        var scheme = Uri.UriSchemeHttp;
        return $"{scheme}://{site.Domain}{site.Path}";
    }

    /// <summary>
    /// Retrieves site details by ID or domain and path.
    /// </summary>
    public Site GetSiteDetails(object fields, bool getAll = true)
    {
        if (fields is int blogId)
        {
            return GetAllSiteDetails(blogId, getAll);
        }

        if (fields is Dictionary<string, string> fieldDict && fieldDict.ContainsKey("domain") && fieldDict.ContainsKey("path"))
        {
            var domain = fieldDict["domain"];
            var path = fieldDict["path"];
            return _context.Sites.FirstOrDefault(s =>
                s.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase) &&
                s.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        }

        return null;
    }

    /// <summary>
    /// Updates site details in the database.
    /// </summary>
    public bool UpdateSiteDetails(int blogId, Dictionary<string, object> details)
    {
        var site = _context.Sites.FirstOrDefault(s => s.BlogId == blogId);
        if (site == null)
        {
            return false;
        }

        foreach (var detail in details)
        {
            if (detail.Key == "last_updated")
            {
                site.LastUpdated = Convert.ToDateTime(detail.Value);
            }
            else
            {
                site.Meta[detail.Key] = detail.Value;
            }
        }

        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Clears the cache for site details.
    /// </summary>
    public void ClearSiteCache(int blogId)
    {
        // Placeholder for cache clearing logic
        Console.WriteLine($"Cache cleared for site with BlogId: {blogId}");
    }

    /// <summary>
    /// Retrieves recently updated sites.
    /// </summary>
    public List<Site> GetRecentlyUpdatedSites(int start = 0, int quantity = 10)
    {
        return _context.Sites
            .Where(s => s.Public && !s.Archived && !s.Spam && !s.Deleted && s.LastUpdated != DateTime.MinValue)
            .OrderByDescending(s => s.LastUpdated)
            .Skip(start)
            .Take(quantity)
            .ToList();
    }

    /// <summary>
    /// Switches to another site context.
    /// </summary>
    public void SwitchToSite(int newSiteId, int oldSiteId)
    {
        if (newSiteId == oldSiteId)
        {
            return;
        }

        // Placeholder for switching roles and user capabilities
        Console.WriteLine($"Switched from site {oldSiteId} to site {newSiteId}");
    }

    /// <summary>
    /// Adds an option to a site.
    /// </summary>
    public bool AddSiteOption(int blogId, string optionName, object value)
    {
        var site = _context.Sites.FirstOrDefault(s => s.BlogId == blogId);
        if (site == null)
        {
            return false;
        }

        site.Meta[optionName] = value;
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Deletes an option from a site.
    /// </summary>
    public bool DeleteSiteOption(int blogId, string optionName)
    {
        var site = _context.Sites.FirstOrDefault(s => s.BlogId == blogId);
        if (site == null || !site.Meta.ContainsKey(optionName))
        {
            return false;
        }

        site.Meta.Remove(optionName);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Retrieves all details for a site.
    /// </summary>
    private Site GetAllSiteDetails(int blogId, bool getAll)
    {
        var site = _context.Sites.FirstOrDefault(s => s.BlogId == blogId);
        if (site == null)
        {
            return null;
        }

        return getAll ? site : new Site
        {
            BlogId = site.BlogId,
            Domain = site.Domain,
            Path = site.Path
        };
    }
}