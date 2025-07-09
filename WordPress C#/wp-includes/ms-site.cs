using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class MsSiteService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MsSiteService> _logger;

    public MsSiteService(ApplicationDbContext context, ILogger<MsSiteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a site by its ID.
    /// </summary>
    public Site GetSite(int siteId)
    {
        return _context.Sites.Include(s => s.Meta).FirstOrDefault(s => s.BlogId == siteId);
    }

    /// <summary>
    /// Inserts a new site into the database.
    /// </summary>
    public Site InsertSite(Site site)
    {
        if (site == null)
        {
            throw new ArgumentNullException(nameof(site));
        }

        _context.Sites.Add(site);
        _context.SaveChanges();
        return site;
    }

    /// <summary>
    /// Updates an existing site in the database.
    /// </summary>
    public Site UpdateSite(Site updatedSite)
    {
        var existingSite = _context.Sites.Find(updatedSite.BlogId);
        if (existingSite == null)
        {
            throw new InvalidOperationException("Site not found.");
        }

        // Update fields
        existingSite.Domain = updatedSite.Domain;
        existingSite.Path = updatedSite.Path;
        existingSite.Public = updatedSite.Public;
        existingSite.Archived = updatedSite.Archived;
        existingSite.Mature = updatedSite.Mature;
        existingSite.Spam = updatedSite.Spam;
        existingSite.Deleted = updatedSite.Deleted;
        existingSite.LastUpdated = DateTime.UtcNow;

        _context.SaveChanges();
        return existingSite;
    }

    /// <summary>
    /// Deletes a site from the database.
    /// </summary>
    public void DeleteSite(int siteId)
    {
        var site = _context.Sites.Find(siteId);
        if (site == null)
        {
            throw new InvalidOperationException("Site not found.");
        }

        // Trigger pre-deletion hooks
        OnBeforeDeleteSite(site);

        // Drop tables and delete files
        DropSiteTables(site);
        DeleteSiteFiles(site);

        // Remove from database
        _context.Sites.Remove(site);
        _context.SaveChanges();

        // Trigger post-deletion hooks
        OnAfterDeleteSite(site);
    }

    /// <summary>
    /// Adds or updates metadata for a site.
    /// </summary>
    public void UpdateSiteMeta(int siteId, string metaKey, object metaValue, object prevValue = null)
    {
        var meta = _context.SiteMeta.FirstOrDefault(m => m.SiteId == siteId && m.MetaKey == metaKey);
        if (meta != null)
        {
            if (prevValue != null && !meta.MetaValue.Equals(prevValue))
            {
                return; // Skip update if previous value does not match
            }

            meta.MetaValue = metaValue.ToString();
        }
        else
        {
            meta = new SiteMeta
            {
                SiteId = siteId,
                MetaKey = metaKey,
                MetaValue = metaValue.ToString()
            };
            _context.SiteMeta.Add(meta);
        }

        _context.SaveChanges();
    }

    /// <summary>
    /// Sets the last changed time for the 'sites' cache group.
    /// </summary>
    public void SetSitesLastChanged()
    {
        // Placeholder logic for cache invalidation
        Console.WriteLine("Invalidating cache for 'sites' group...");
    }

    /// <summary>
    /// Handles pre-deletion hooks for a site.
    /// </summary>
    private void OnBeforeDeleteSite(Site site)
    {
        _logger.LogInformation($"Triggering pre-deletion hooks for site ID: {site.BlogId}");
        // Example: Trigger events like 'make_spam_blog', 'archive_blog', etc.
    }

    /// <summary>
    /// Handles post-deletion hooks for a site.
    /// </summary>
    private void OnAfterDeleteSite(Site site)
    {
        _logger.LogInformation($"Triggering post-deletion hooks for site ID: {site.BlogId}");
        // Example: Trigger events like 'unarchive_blog', 'make_ham_blog', etc.
    }

    /// <summary>
    /// Drops all tables associated with a site.
    /// </summary>
    private void DropSiteTables(Site site)
    {
        _logger.LogInformation($"Dropping tables for site ID: {site.BlogId}");
        // Example: SQL commands to drop tables
    }

    /// <summary>
    /// Deletes all files associated with a site.
    /// </summary>
    private void DeleteSiteFiles(Site site)
    {
        _logger.LogInformation($"Deleting files for site ID: {site.BlogId}");
        // Example: File system operations to delete files
    }
}