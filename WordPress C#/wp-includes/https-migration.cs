using System;
using System.Linq;

public class HttpsMigrationService
{
    private readonly ApplicationDbContext _context;

    public HttpsMigrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Checks whether WordPress should replace old HTTP URLs with their HTTPS counterpart.
    /// </summary>
    public bool ShouldReplaceInsecureHomeUrl()
    {
        var settings = GetSiteSettings();

        bool shouldReplace = IsUsingHttps() &&
                             settings.HttpsMigrationRequired &&
                             ParseUrlHost(settings.HomeUrl) == ParseUrlHost(settings.SiteUrl);

        return ApplyFilters("wp_should_replace_insecure_home_url", shouldReplace);
    }

    /// <summary>
    /// Replaces insecure HTTP URLs in the given content with their HTTPS counterpart.
    /// </summary>
    public string ReplaceInsecureHomeUrl(string content)
    {
        if (!ShouldReplaceInsecureHomeUrl())
        {
            return content;
        }

        var settings = GetSiteSettings();
        var httpsUrl = EnsureHttpsScheme(settings.HomeUrl);
        var httpUrl = EnsureHttpScheme(httpsUrl);

        // Replace both normal and escaped URLs
        var escapedHttpsUrl = httpsUrl.Replace("/", "\\/");
        var escapedHttpUrl = httpUrl.Replace("/", "\\/");

        return content.Replace(httpUrl, httpsUrl).Replace(escapedHttpUrl, escapedHttpsUrl);
    }

    /// <summary>
    /// Updates the 'home' and 'siteurl' options to use the HTTPS variant of their URL.
    /// </summary>
    public HttpsMigrationResult UpdateUrlsToHttps()
    {
        var settings = GetSiteSettings();

        // Backup original URLs
        var origHome = settings.HomeUrl;
        var origSiteUrl = settings.SiteUrl;

        // Convert to HTTPS
        settings.HomeUrl = EnsureHttpsScheme(origHome);
        settings.SiteUrl = EnsureHttpsScheme(origSiteUrl);

        // Update options
        UpdateSiteSettings(settings);

        if (!IsUsingHttps())
        {
            // Revert changes if HTTPS is not recognized
            settings.HomeUrl = origHome;
            settings.SiteUrl = origSiteUrl;
            UpdateSiteSettings(settings);

            return new HttpsMigrationResult
            {
                Success = false,
                Message = "Failed to recognize HTTPS. Changes reverted."
            };
        }

        return new HttpsMigrationResult
        {
            Success = true,
            Message = "URLs successfully updated to HTTPS."
        };
    }

    /// <summary>
    /// Updates the 'https_migration_required' option if needed when the URL has been updated from HTTP to HTTPS.
    /// </summary>
    public void UpdateHttpsMigrationRequired(string oldUrl, string newUrl)
    {
        if (IsInstalling())
        {
            return;
        }

        var normalizedOldUrl = Untrailingslashit(oldUrl);
        var normalizedNewUrl = Untrailingslashit(newUrl);

        if (normalizedOldUrl != EnsureHttpScheme(normalizedNewUrl))
        {
            DeleteOption("https_migration_required");
            return;
        }

        var freshSite = GetFreshSiteStatus();
        var httpsMigrationRequired = !freshSite;

        UpdateOption("https_migration_required", httpsMigrationRequired);
    }

    /// <summary>
    /// Checks whether the site is using HTTPS.
    /// </summary>
    private bool IsUsingHttps()
    {
        var settings = GetSiteSettings();
        return ParseUrlScheme(settings.HomeUrl) == "https" && ParseUrlScheme(settings.SiteUrl) == "https";
    }

    /// <summary>
    /// Parses the host part of a URL.
    /// </summary>
    private string ParseUrlHost(string url)
    {
        var uri = new Uri(url);
        return uri.Host;
    }

    /// <summary>
    /// Ensures the URL uses the HTTPS scheme.
    /// </summary>
    private string EnsureHttpsScheme(string url)
    {
        var uri = new Uri(url);
        return new UriBuilder(uri) { Scheme = "https", Port = -1 }.ToString();
    }

    /// <summary>
    /// Ensures the URL uses the HTTP scheme.
    /// </summary>
    private string EnsureHttpScheme(string url)
    {
        var uri = new Uri(url);
        return new UriBuilder(uri) { Scheme = "http", Port = -1 }.ToString();
    }

    /// <summary>
    /// Removes trailing slashes from a URL.
    /// </summary>
    private string Untrailingslashit(string url)
    {
        return url.TrimEnd('/');
    }

    /// <summary>
    /// Retrieves the site settings.
    /// </summary>
    private SiteSettings GetSiteSettings()
    {
        return _context.SiteSettings.FirstOrDefault() ?? new SiteSettings();
    }

    /// <summary>
    /// Updates the site settings.
    /// </summary>
    private void UpdateSiteSettings(SiteSettings settings)
    {
        _context.Update(settings);
        _context.SaveChanges();
    }

    /// <summary>
    /// Deletes an option.
    /// </summary>
    private void DeleteOption(string optionName)
    {
        var option = _context.Options.FirstOrDefault(o => o.Name == optionName);
        if (option != null)
        {
            _context.Options.Remove(option);
            _context.SaveChanges();
        }
    }

    /// <summary>
    /// Updates an option.
    /// </summary>
    private void UpdateOption(string optionName, object value)
    {
        var option = _context.Options.FirstOrDefault(o => o.Name == optionName);
        if (option == null)
        {
            option = new Option { Name = optionName, Value = value.ToString() };
            _context.Options.Add(option);
        }
        else
        {
            option.Value = value.ToString();
        }
        _context.SaveChanges();
    }

    /// <summary>
    /// Applies filters to a value.
    /// </summary>
    private T ApplyFilters<T>(string filterName, T value)
    {
        // Placeholder for filter logic
        return value;
    }

    /// <summary>
    /// Checks if WordPress is being installed.
    /// </summary>
    private bool IsInstalling()
    {
        // Placeholder for installation check logic
        return false;
    }

    /// <summary>
    /// Checks if this is a fresh site.
    /// </summary>
    private bool GetFreshSiteStatus()
    {
        // Placeholder for fresh site status logic
        return false;
    }
}