using System;
using Microsoft.Extensions.Logging;

public class MsLoadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MsLoadService> _logger;

    public MsLoadService(ApplicationDbContext context, ILogger<MsLoadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Checks if a subdomain configuration is enabled.
    /// </summary>
    public bool IsSubdomainInstall()
    {
        return Environment.GetEnvironmentVariable("SUBDOMAIN_INSTALL") != null || 
               Environment.GetEnvironmentVariable("VHOST") == "yes";
    }

    /// <summary>
    /// Loads the current network and site based on domain and path.
    /// </summary>
    public (Network CurrentNetwork, Site CurrentSite) LoadCurrentNetworkAndSite(string domain, string path)
    {
        // Check if the network is defined in wp-config.php
        var definedNetwork = GetDefinedNetwork();
        if (definedNetwork != null)
        {
            var currentSite = GetSiteByPath(definedNetwork, domain, path);
            return (definedNetwork, currentSite);
        }

        // Search for the network and site in the database
        var network = FindNetworkByDomainAndPath(domain, path);
        if (network == null)
        {
            _logger.LogWarning($"Network not found for domain: {domain}, path: {path}");
            return (null, null);
        }

        var site = GetSiteByPath(network, domain, path);
        if (site == null)
        {
            _logger.LogWarning($"Site not found for domain: {domain}, path: {path}");
            return (network, null);
        }

        return (network, site);
    }

    /// <summary>
    /// Retrieves the network defined in wp-config.php.
    /// </summary>
    private Network GetDefinedNetwork()
    {
        var domain = Environment.GetEnvironmentVariable("DOMAIN_CURRENT_SITE");
        var path = Environment.GetEnvironmentVariable("PATH_CURRENT_SITE");
        if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(path))
        {
            return null;
        }

        return new Network
        {
            Id = int.Parse(Environment.GetEnvironmentVariable("SITE_ID_CURRENT_SITE") ?? "1"),
            Domain = domain,
            Path = path,
            BlogId = int.Parse(Environment.GetEnvironmentVariable("BLOG_ID_CURRENT_SITE") ?? "1")
        };
    }

    /// <summary>
    /// Finds a network by domain and path.
    /// </summary>
    private Network FindNetworkByDomainAndPath(string domain, string path)
    {
        return _context.Networks.FirstOrDefault(n =>
            n.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase) &&
            n.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Retrieves a site by domain and path.
    /// </summary>
    private Site GetSiteByPath(Network network, string domain, string path)
    {
        return _context.Sites.FirstOrDefault(s =>
            s.SiteId == network.Id &&
            s.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase) &&
            s.Path.StartsWith(path, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks the status of the current site.
    /// </summary>
    public bool CheckSiteStatus(Site site)
    {
        if (site.Deleted)
        {
            HandleDeletedSite(site);
            return false;
        }

        if (site.Archived || site.Spam)
        {
            HandleArchivedOrSpammedSite(site);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Handles a deleted site.
    /// </summary>
    private void HandleDeletedSite(Site site)
    {
        _logger.LogWarning($"Site with ID {site.BlogId} is deleted.");
        // Redirect to a custom error page or show a message
    }

    /// <summary>
    /// Handles an archived or spammed site.
    /// </summary>
    private void HandleArchivedOrSpammedSite(Site site)
    {
        _logger.LogWarning($"Site with ID {site.BlogId} is archived or spammed.");
        // Redirect to a custom error page or show a message
    }
}