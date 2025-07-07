using System;
using Microsoft.Extensions.Logging;

public class MsDeprecatedService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MsDeprecatedService> _logger;

    public MsDeprecatedService(ApplicationDbContext context, ILogger<MsDeprecatedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Deprecated function to get the dashboard blog.
    /// </summary>
    [Obsolete("Use GetSite() instead.")]
    public Site GetDashboardBlog()
    {
        _logger.LogWarning("The 'GetDashboardBlog' method is deprecated. Use 'GetSite()' instead.");
        var dashboardBlogId = _context.SiteOptions.FirstOrDefault(s => s.Key == "dashboard_blog")?.Value;
        return dashboardBlogId != null ? _context.Sites.Find(int.Parse(dashboardBlogId)) : _context.Sites.Find(1);
    }

    /// <summary>
    /// Deprecated function to generate a random password.
    /// </summary>
    [Obsolete("Use GeneratePassword() instead.")]
    public string GenerateRandomPassword(int length = 8)
    {
        _logger.LogWarning("The 'GenerateRandomPassword' method is deprecated. Use 'GeneratePassword()' instead.");
        return GeneratePassword(length);
    }

    /// <summary>
    /// Generates a random password.
    /// </summary>
    public string GeneratePassword(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Deprecated function to get the most active blogs.
    /// </summary>
    [Obsolete("This function is deprecated and should not be used.")]
    public List<Site> GetMostActiveBlogs(int count = 10, bool display = true)
    {
        _logger.LogWarning("The 'GetMostActiveBlogs' method is deprecated.");
        return _context.Sites
            .Where(s => !s.Archived && !s.Spam && !s.Deleted)
            .OrderByDescending(s => s.PostCount)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Deprecated function to redirect with an updated parameter.
    /// </summary>
    [Obsolete("Use AddQueryParameter() instead.")]
    public string WpmuAdminRedirectAddUpdatedParam(string url = "")
    {
        _logger.LogWarning("The 'WpmuAdminRedirectAddUpdatedParam' method is deprecated. Use 'AddQueryParameter()' instead.");
        return AddQueryParameter(url, "updated", "true");
    }

    /// <summary>
    /// Adds a query parameter to a URL.
    /// </summary>
    public string AddQueryParameter(string url, string key, string value)
    {
        var uriBuilder = new UriBuilder(url);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query[key] = value;
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    /// <summary>
    /// Deprecated function to insert a new blog.
    /// </summary>
    [Obsolete("Use InsertSite() instead.")]
    public int InsertBlog(string domain, string path, int siteId)
    {
        _logger.LogWarning("The 'InsertBlog' method is deprecated. Use 'InsertSite()' instead.");
        return InsertSite(domain, path, siteId);
    }

    /// <summary>
    /// Inserts a new site into the database.
    /// </summary>
    public int InsertSite(string domain, string path, int siteId)
    {
        var site = new Site
        {
            Domain = domain,
            Path = path,
            BlogId = siteId,
            LastUpdated = DateTime.UtcNow
        };

        _context.Sites.Add(site);
        _context.SaveChanges();
        return site.BlogId;
    }

    /// <summary>
    /// Deprecated function to install a blog.
    /// </summary>
    [Obsolete("This function is deprecated and should not be used.")]
    public void InstallBlog(int blogId, string blogTitle = "")
    {
        _logger.LogWarning("The 'InstallBlog' method is deprecated.");
        var site = _context.Sites.Find(blogId);
        if (site == null)
        {
            throw new ArgumentException("Site not found.");
        }

        site.Public = true;
        site.LastUpdated = DateTime.UtcNow;
        _context.SaveChanges();
    }

    /// <summary>
    /// Deprecated function to check if a user is a super admin.
    /// </summary>
    [Obsolete("Use IsSuperAdmin() instead.")]
    public bool IsMainAdmin(int userId)
    {
        _logger.LogWarning("The 'IsMainAdmin' method is deprecated. Use 'IsSuperAdmin()' instead.");
        return IsSuperAdmin(userId);
    }

    /// <summary>
    /// Checks if a user is a super admin.
    /// </summary>
    public bool IsSuperAdmin(int userId)
    {
        var user = _context.Users.Find(userId);
        return user?.IsSuperAdmin ?? false;
    }

    /// <summary>
    /// Deprecated function to gracefully fail.
    /// </summary>
    [Obsolete("Use HandleError() instead.")]
    public void GracefulFail(string message)
    {
        _logger.LogWarning("The 'GracefulFail' method is deprecated. Use 'HandleError()' instead.");
        HandleError(message);
    }

    /// <summary>
    /// Handles an error by logging and displaying a message.
    /// </summary>
    public void HandleError(string message)
    {
        _logger.LogError(message);
        Console.WriteLine($"Error: {message}");
    }
}