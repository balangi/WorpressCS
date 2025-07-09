using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class MsSettingsService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MsSettingsService> _logger;

    public MsSettingsService(ApplicationDbContext context, IMemoryCache cache, ILogger<MsSettingsService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the current network and site based on the request.
    /// </summary>
    public void InitializeCurrentNetworkAndSite(HttpContext context)
    {
        // Check if the network and site are already loaded via a custom sunrise file.
        var currentNetwork = GetCurrentNetwork();
        var currentSite = GetCurrentSite();

        if (currentNetwork == null || currentSite == null)
        {
            var domain = GetDomainFromRequest(context);
            var path = GetPathFromRequest(context);

            var bootstrapResult = LoadCurrentNetworkAndSite(domain, path, IsSubdomainInstall());

            if (bootstrapResult == BootstrapResult.Success)
            {
                currentNetwork = GetCurrentNetwork();
                currentSite = GetCurrentSite();
            }
            else if (bootstrapResult == BootstrapResult.Failure)
            {
                HandleNotInstalled(domain, path);
                return;
            }
            else
            {
                Redirect(bootstrapResult.RedirectUrl);
                return;
            }
        }

        // Set global variables for backward compatibility.
        SetGlobalVariables(currentSite);

        // Define upload directory constants.
        DefineUploadConstants();
    }

    /// <summary>
    /// Retrieves the current network from the cache or database.
    /// </summary>
    private Network GetCurrentNetwork()
    {
        return _cache.Get<Network>("current_network") ?? _context.Networks.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the current site from the cache or database.
    /// </summary>
    private Site GetCurrentSite()
    {
        return _cache.Get<Site>("current_site") ?? _context.Sites.FirstOrDefault();
    }

    /// <summary>
    /// Extracts the domain from the HTTP request.
    /// </summary>
    private string GetDomainFromRequest(HttpContext context)
    {
        var domain = context.Request.Host.Host.ToLowerInvariant();
        if (domain.EndsWith(":80"))
        {
            domain = domain.Substring(0, domain.Length - 3);
        }
        else if (domain.EndsWith(":443"))
        {
            domain = domain.Substring(0, domain.Length - 4);
        }
        return domain;
    }

    /// <summary>
    /// Extracts the path from the HTTP request.
    /// </summary>
    private string GetPathFromRequest(HttpContext context)
    {
        var path = context.Request.Path.ToString();
        if (IsAdminRequest(context))
        {
            path = System.Text.RegularExpressions.Regex.Replace(path, "(.*)/wp-admin/.*", "$1/");
        }
        return path.Split('?')[0];
    }

    /// <summary>
    /// Checks if the current request is for the admin area.
    /// </summary>
    private bool IsAdminRequest(HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/wp-admin");
    }

    /// <summary>
    /// Loads the current network and site based on the domain and path.
    /// </summary>
    private BootstrapResult LoadCurrentNetworkAndSite(string domain, string path, bool isSubdomainInstall)
    {
        var network = FindNetworkByDomainAndPath(domain, path);
        if (network == null)
        {
            return BootstrapResult.Failure;
        }

        var site = FindSiteByPath(network, domain, path);
        if (site == null)
        {
            var redirectUrl = DetermineRedirectUrl(domain, path, isSubdomainInstall);
            return new BootstrapResult { RedirectUrl = redirectUrl };
        }

        _cache.Set("current_network", network);
        _cache.Set("current_site", site);

        return BootstrapResult.Success;
    }

    /// <summary>
    /// Finds a network by domain and path.
    /// </summary>
    private Network FindNetworkByDomainAndPath(string domain, string path)
    {
        return _context.Networks.FirstOrDefault(n =>
            n.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase) &&
            n.Path.StartsWith(path, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Finds a site by domain and path.
    /// </summary>
    private Site FindSiteByPath(Network network, string domain, string path)
    {
        return _context.Sites.FirstOrDefault(s =>
            s.SiteId == network.Id &&
            s.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase) &&
            s.Path.StartsWith(path, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines the redirect URL when no site is found.
    /// </summary>
    private string DetermineRedirectUrl(string domain, string path, bool isSubdomainInstall)
    {
        if (isSubdomainInstall)
        {
            return $"https://{domain}/wp-signup.php?new={domain}";
        }
        return $"https://{domain}{path}";
    }

    /// <summary>
    /// Handles the case where no network or site is installed.
    /// </summary>
    private void HandleNotInstalled(string domain, string path)
    {
        _logger.LogError($"No network or site found for domain: {domain}, path: {path}");
        throw new Exception("Multisite is not installed.");
    }

    /// <summary>
    /// Redirects to the specified URL.
    /// </summary>
    private void Redirect(string url)
    {
        Console.WriteLine($"Redirecting to: {url}");
        // Implement actual redirection logic here.
    }

    /// <summary>
    /// Sets global variables for backward compatibility.
    /// </summary>
    private void SetGlobalVariables(Site site)
    {
        Environment.SetEnvironmentVariable("DOMAIN", site.Domain);
        Environment.SetEnvironmentVariable("PATH", site.Path);
        Environment.SetEnvironmentVariable("SITE_ID", site.SiteId.ToString());
        Environment.SetEnvironmentVariable("PUBLIC", site.Public.ToString());
    }

    /// <summary>
    /// Defines upload directory constants.
    /// </summary>
    private void DefineUploadConstants()
    {
        const string DefaultUploadBlogsDir = "wp-content/blogs.dir";
        var siteId = GetCurrentBlogId();

        Environment.SetEnvironmentVariable("UPLOADBLOGSDIR", DefaultUploadBlogsDir);
        Environment.SetEnvironmentVariable("UPLOADS", $"{DefaultUploadBlogsDir}/{siteId}/files/");
    }

    /// <summary>
    /// Retrieves the current blog ID.
    /// </summary>
    private int GetCurrentBlogId()
    {
        return _cache.Get<int>("current_blog_id");
    }

    /// <summary>
    /// Checks if a subdomain configuration is enabled.
    /// </summary>
    private bool IsSubdomainInstall()
    {
        return Environment.GetEnvironmentVariable("SUBDOMAIN_INSTALL") != null ||
               Environment.GetEnvironmentVariable("VHOST") == "yes";
    }
}

/// <summary>
/// Represents the result of the bootstrap process.
/// </summary>
public class BootstrapResult
{
    public static BootstrapResult Success => new BootstrapResult { Status = "Success" };
    public static BootstrapResult Failure => new BootstrapResult { Status = "Failure" };