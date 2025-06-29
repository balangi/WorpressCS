using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

public class HttpsDetectionService
{
    private readonly ApplicationDbContext _context;

    public HttpsDetectionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Checks whether the website is using HTTPS.
    /// </summary>
    public bool IsUsingHttps()
    {
        return IsHomeUrlUsingHttps() && IsSiteUrlUsingHttps();
    }

    /// <summary>
    /// Checks whether the home URL is using HTTPS.
    /// </summary>
    public bool IsHomeUrlUsingHttps()
    {
        var homeUrl = GetSiteSettings().HomeUrl;
        return ParseUrlScheme(homeUrl) == "https";
    }

    /// <summary>
    /// Checks whether the site URL is using HTTPS.
    /// </summary>
    public bool IsSiteUrlUsingHttps()
    {
        var siteUrl = GetFilteredSiteUrl();
        return ParseUrlScheme(siteUrl) == "https";
    }

    /// <summary>
    /// Checks whether HTTPS is supported for the server and domain.
    /// </summary>
    public async Task<HttpsSupportResult> IsHttpsSupportedAsync()
    {
        var errors = await GetHttpsDetectionErrorsAsync();
        return new HttpsSupportResult
        {
            IsHttpsSupported = !errors.Any(),
            Errors = errors
        };
    }

    /// <summary>
    /// Runs a remote HTTPS request to detect whether HTTPS is supported, and stores potential errors.
    /// </summary>
    private async Task<List<HttpsDetectionError>> GetHttpsDetectionErrorsAsync()
    {
        var errors = new List<HttpsDetectionError>();

        // Make an HTTPS request to the home URL
        var homeUrl = GetSiteSettings().HomeUrl;
        var httpsUrl = EnsureHttpsScheme(homeUrl);

        var response = await SendHttpRequestAsync(httpsUrl, true);
        if (response.IsError)
        {
            var unverifiedResponse = await SendHttpRequestAsync(httpsUrl, false);
            if (unverifiedResponse.IsError)
            {
                errors.Add(new HttpsDetectionError
                {
                    Code = "https_request_failed",
                    Message = "HTTPS request failed."
                });
            }
            else
            {
                errors.Add(new HttpsDetectionError
                {
                    Code = "ssl_verification_failed",
                    Message = "SSL verification failed."
                });
            }
        }

        if (!response.IsError)
        {
            if (response.StatusCode != 200)
            {
                errors.Add(new HttpsDetectionError
                {
                    Code = "bad_response_code",
                    Message = $"Bad response code: {response.StatusCode}"
                });
            }
            else if (!IsLocalHtmlOutput(response.Body))
            {
                errors.Add(new HttpsDetectionError
                {
                    Code = "bad_response_source",
                    Message = "It looks like the response did not come from this site."
                });
            }
        }

        return errors;
    }

    /// <summary>
    /// Sends an HTTP request and retrieves the response.
    /// </summary>
    private async Task<(bool IsError, int StatusCode, string Body)> SendHttpRequestAsync(string url, bool sslVerify)
    {
        using var httpClient = new HttpClient();
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-Control", "no-cache");

            var response = await httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            return (false, (int)response.StatusCode, body);
        }
        catch
        {
            return (true, 0, null);
        }
    }

    /// <summary>
    /// Checks whether a given HTML string is likely an output from this WordPress site.
    /// </summary>
    private bool IsLocalHtmlOutput(string html)
    {
        // Check for RSD link
        if (HasAction("wp_head", "rsd_link"))
        {
            var rsdPattern = RemoveScheme(GetRestUrl("xmlrpc.php?rsd", "rpc"));
            return html.Contains(rsdPattern, StringComparison.OrdinalIgnoreCase);
        }

        // Check for REST API link
        if (HasAction("wp_head", "rest_output_link_wp_head"))
        {
            var restPattern = RemoveScheme(GetRestUrl());
            return html.Contains(restPattern, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    /// <summary>
    /// Parses the URL scheme from a given URL.
    /// </summary>
    private string ParseUrlScheme(string url)
    {
        var uri = new Uri(url);
        return uri.Scheme;
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
    /// Removes the scheme (http/https) from a URL.
    /// </summary>
    private string RemoveScheme(string url)
    {
        return url.Replace("http://", "").Replace("https://", "");
    }

    /// <summary>
    /// Retrieves filtered site URL.
    /// </summary>
    private string GetFilteredSiteUrl()
    {
        var siteUrl = GetSiteSettings().SiteUrl;
        return ApplyFilters("site_url", siteUrl, "", null, null);
    }

    /// <summary>
    /// Retrieves the site settings.
    /// </summary>
    private SiteSettings GetSiteSettings()
    {
        return _context.SiteSettings.FirstOrDefault() ?? new SiteSettings();
    }

    /// <summary>
    /// Applies filters to a value.
    /// </summary>
    private string ApplyFilters(string filterName, string value, string param1, object param2, object param3)
    {
        // Placeholder for filter logic
        return value;
    }

    /// <summary>
    /// Checks if a specific action exists.
    /// </summary>
    private bool HasAction(string hookName, string actionName)
    {
        // Placeholder for action check logic
        return true;
    }

    /// <summary>
    /// Retrieves the REST URL.
    /// </summary>
    private string GetRestUrl(string path = "", string scheme = "")
    {
        // Placeholder for REST URL logic
        return "https://example.com/rest-api ";
    }
}