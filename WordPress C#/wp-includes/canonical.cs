// Services/CanonicalRedirectService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class CanonicalRedirectService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUrlService _urlService;
    private readonly IRewriteService _rewriteService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CanonicRedirectService(
        ApplicationDbContext dbContext,
        IUrlService urlService,
        IRewriteService rewriteService,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _urlService = urlService;
        _rewriteService = rewriteService;
        _httpContextAccessor = httpContextAccessor;
    }

    public string RedirectCanonical(string requestedUrl = null, bool doRedirect = true)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var request = httpContext.Request;

        // Check request method
        if (!string.Equals(request.Method, "GET", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(request.Method, "HEAD", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // Skip for admin, search, preview, trackback, etc.
        if (IsAdmin() || IsSearch() || IsPreview() || IsTrackback() || IsFavicon())
        {
            return null;
        }

        // Build requested URL if not provided
        if (string.IsNullOrEmpty(requestedUrl))
        {
            var scheme = request.IsHttps ? "https://" : "http://";
            requestedUrl = $"{scheme}{request.Host}{request.Path}{request.QueryString}";
        }

        // Parse URLs
        var originalUri = new Uri(requestedUrl);
        var redirectUri = new UriBuilder(originalUri);
        string redirectUrl = null;
        object redirectObj = null;

        // Handle feeds
        if (IsFeed() && GetQueryVar("p") > 0)
        {
            int postId = GetQueryVar("p");
            redirectUrl = _urlService.GetPostCommentsFeedLink(postId, GetQueryVar("feed"));
            redirectObj = _dbContext.Posts.Find(postId);

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                redirectUri.Query = RemoveQueryArgsIfNotInUrl(
                    redirectUri.Query,
                    new[] { "p", "page_id", "attachment_id", "pagename", "name", "post_type", "feed" },
                    redirectUrl);
                redirectUri.Path = new Uri(redirectUrl).AbsolutePath;
            }
        }

        // Handle singular requests with no posts
        if (IsSingular() && GetQueryVar("p") > 0)
        {
            int postId = GetQueryVar("p");
            var post = _dbContext.Posts
                .Where(p => p.Id == postId)
                .Select(p => new { p.PostType, p.PostParent })
                .FirstOrDefault();

            if (post != null)
            {
                if (post.PostType == "revision" && post.PostParent > 0)
                {
                    postId = post.PostParent;
                }

                redirectUrl = _urlService.GetPermalink(postId);
                redirectObj = _dbContext.Posts.Find(postId);

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUri.Query = RemoveQueryArgsIfNotInUrl(
                        redirectUri.Query,
                        new[] { "p", "page_id", "attachment_id", "pagename", "name", "post_type" },
                        redirectUrl);
                }
            }
        }

        // Handle 404s
        if (Is404())
        {
            // Try to find post by ID
            int postId = new[] { GetQueryVar("p"), GetQueryVar("page_id"), GetQueryVar("attachment_id") }.Max();
            if (postId > 0)
            {
                var post = _dbContext.Posts.Find(postId);
                if (post != null && IsPostStatusViewable(post.PostStatus) && IsPostTypeViewable(post.PostType))
                {
                    redirectUrl = _urlService.GetPermalink(postId);
                    redirectObj = post;

                    redirectUri.Query = RemoveQueryArgsIfNotInUrl(
                        redirectUri.Query,
                        new[] { "p", "page_id", "attachment_id", "pagename", "name", "post_type" },
                        redirectUrl);
                }
            }

            // Validate date
            int year = GetQueryVar("year");
            int month = GetQueryVar("monthnum");
            int day = GetQueryVar("day");

            if (year > 0 && month > 0 && day > 0)
            {
                if (!IsValidDate(month, day, year))
                {
                    redirectUrl = _urlService.GetMonthLink(year, month);
                    redirectUri.Query = RemoveQueryArgsIfNotInUrl(
                        redirectUri.Query,
                        new[] { "year", "monthnum", "day" },
                        redirectUrl);
                }
            }
            else if (year > 0 && month > 12)
            {
                redirectUrl = _urlService.GetYearLink(year);
                redirectUri.Query = RemoveQueryArgsIfNotInUrl(
                    redirectUri.Query,
                    new[] { "year", "monthnum" },
                    redirectUrl);
            }

            // Handle pagination
            if (GetQueryVar("page") > 0)
            {
                postId = 0;
                // Logic to get post ID from query...
                
                if (postId > 0)
                {
                    redirectUrl = _urlService.GetPermalink(postId);
                    redirectObj = _dbContext.Posts.Find(postId);

                    redirectUri.Path = redirectUri.Path.TrimEnd((GetQueryVar("page") + "/").ToCharArray());
                    redirectUri.Query = RemoveQueryArg(redirectUri.Query, "page");
                }
            }

            // Try to guess permalink
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = RedirectGuess404Permalink();
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUri.Query = RemoveQueryArgsIfNotInUrl(
                        redirectUri.Query,
                        new[] { "page", "feed", "p", "page_id", "attachment_id", "pagename", "name", "post_type" },
                        redirectUrl);
                }
            }
        }

        // Handle attachments
        if (IsAttachment() && !GetOption("wp_attachment_pages_enabled", true))
        {
            int attachmentId = GetQueryVar("attachment_id");
            var attachment = _dbContext.Posts.Find(attachmentId);
            
            if (attachment != null)
            {
                string attachmentUrl = GetAttachmentUrl(attachmentId);
                if (attachmentUrl != redirectUrl)
                {
                    if (attachment.PostParent > 0)
                    {
                        redirectObj = _dbContext.Posts.Find(attachment.PostParent);
                    }
                    redirectUrl = attachmentUrl;
                }
            }
        }

        // Finalize redirect URL
        if (!string.IsNullOrEmpty(redirectUrl))
        {
            var newUri = new Uri(redirectUrl);
            redirectUri.Scheme = newUri.Scheme;
            redirectUri.Host = newUri.Host;
            redirectUri.Port = newUri.Port;
            redirectUri.Path = newUri.AbsolutePath;
            redirectUri.Query = newUri.Query;
        }

        // Handle trailing slashes and other URL cleanup
        redirectUri.Path = CleanPath(redirectUri.Path);

        // Compare original and redirect URLs
        if (!UrlsAreEqual(originalUri, redirectUri.Uri))
        {
            redirectUrl = redirectUri.Uri.ToString();
        }

        // Check if redirect is needed
        if (string.IsNullOrEmpty(redirectUrl) || 
            StripFragmentFromUrl(redirectUrl) == StripFragmentFromUrl(requestedUrl))
        {
            return null;
        }

        // Apply filters (would be implemented via DI in real scenario)
        // redirectUrl = ApplyFilters("redirect_canonical", redirectUrl, requestedUrl);

        if (doRedirect)
        {
            // Prevent redirect loops
            if (RedirectCanonical(redirectUrl, false) == null)
            {
                httpContext.Response.Redirect(redirectUrl, permanent: true);
                return null;
            }
        }

        return redirectUrl;
    }

    private string CleanPath(string path)
    {
        // Implementation of path cleaning logic
        // (similar to the PHP version with trailing slashes, index.php removal, etc.)
        return path;
    }

    private bool UrlsAreEqual(Uri uri1, Uri uri2)
    {
        // Compare URLs ignoring case and fragments
        return Uri.Compare(uri1, uri2, 
            UriComponents.AbsoluteUri & ~UriComponents.Fragment, 
            UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0;
    }

    private string StripFragmentFromUrl(string url)
    {
        var uri = new Uri(url);
        return new UriBuilder(uri) { Fragment = string.Empty }.Uri.ToString();
    }

    private string RemoveQueryArg(string query, string arg)
    {
        // Implementation to remove query arg
        return query;
    }

    private string RemoveQueryArgsIfNotInUrl(string query, string[] args, string url)
    {
        // Implementation similar to PHP version
        return query;
    }

    private string RedirectGuess404Permalink()
    {
        // Implementation similar to PHP version using LINQ queries
        if (!string.IsNullOrEmpty(GetQueryVar("name")))
        {
            var name = GetQueryVar("name");
            var postTypes = GetPubliclyViewablePostTypes();
            var postStatuses = GetPubliclyViewablePostStatuses();

            var query = _dbContext.Posts.AsQueryable();

            bool strictGuess = false; // Would come from filter in real implementation
            if (strictGuess)
            {
                query = query.Where(p => p.PostName == name);
            }
            else
            {
                query = query.Where(p => p.PostName.StartsWith(name));
            }

            // Add filters based on other query vars
            if (GetQueryVar("year") > 0)
            {
                query = query.Where(p => p.PostDate.Year == GetQueryVar("year"));
            }
            // Similar for month, day, etc.

            var postId = query
                .Where(p => postTypes.Contains(p.PostType) && postStatuses.Contains(p.PostStatus))
                .Select(p => p.Id)
                .FirstOrDefault();

            if (postId > 0)
            {
                if (GetQueryVar("feed") != null)
                {
                    return _urlService.GetPostCommentsFeedLink(postId, GetQueryVar("feed"));
                }
                else if (GetQueryVar("page") > 1)
                {
                    return $"{_urlService.GetPermalink(postId)}/{GetQueryVar("page")}/";
                }
                else
                {
                    return _urlService.GetPermalink(postId);
                }
            }
        }

        return null;
    }

    // Helper methods to mimic WordPress functions
    private bool IsAdmin() { /* ... */ }
    private bool IsSearch() { /* ... */ }
    private bool IsPreview() { /* ... */ }
    private bool Is404() { /* ... */ }
    private bool IsSingular() { /* ... */ }
    private bool IsAttachment() { /* ... */ }
    private bool IsFeed() { /* ... */ }
    private int GetQueryVar(string name) { /* ... */ }
    private T GetOption<T>(string name, T defaultValue = default) { /* ... */ }
    private bool IsPostStatusViewable(string status) { /* ... */ }
    private bool IsPostTypeViewable(string type) { /* ... */ }
    private bool IsValidDate(int month, int day, int year) { /* ... */ }
    private string[] GetPubliclyViewablePostTypes() { /* ... */ }
    private string[] GetPubliclyViewablePostStatuses() { /* ... */ }
    private string GetAttachmentUrl(int attachmentId) { /* ... */ }
}