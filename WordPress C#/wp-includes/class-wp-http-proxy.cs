using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class HttpProxyManager
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HttpProxyManager> _logger;

    public HttpProxyManager(IConfiguration configuration, ILogger<HttpProxyManager> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// بررسی فعال بودن پروکسی
    /// </summary>
    public bool IsEnabled()
    {
        var host = _configuration["ProxySettings:Host"];
        var port = _configuration["ProxySettings:Port"];
        return !string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(port);
    }

    /// <summary>
    /// بررسی نیاز به احراز هویت
    /// </summary>
    public bool UseAuthentication()
    {
        var username = _configuration["ProxySettings:Username"];
        var password = _configuration["ProxySettings:Password"];
        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }

    /// <summary>
    /// دریافت هاست پروکسی
    /// </summary>
    public string Host()
    {
        return _configuration["ProxySettings:Host"] ?? string.Empty;
    }

    /// <summary>
    /// دریافت پورت پروکسی
    /// </summary>
    public string Port()
    {
        return _configuration["ProxySettings:Port"] ?? string.Empty;
    }

    /// <summary>
    /// دریافت نام کاربری پروکسی
    /// </summary>
    public string Username()
    {
        return _configuration["ProxySettings:Username"] ?? string.Empty;
    }

    /// <summary>
    /// دریافت رمز عبور پروکسی
    /// </summary>
    public string Password()
    {
        return _configuration["ProxySettings:Password"] ?? string.Empty;
    }

    /// <summary>
    /// دریافت رشته احراز هویت پروکسی
    /// </summary>
    public string Authentication()
    {
        return $"{Username()}:{Password()}";
    }

    /// <summary>
    /// دریافت هدر احراز هویت پروکسی
    /// </summary>
    public string AuthenticationHeader()
    {
        var authString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Authentication()));
        return $"Proxy-Authorization: Basic {authString}";
    }

    /// <summary>
    /// بررسی ارسال درخواست از طریق پروکسی
    /// </summary>
    public bool SendThroughProxy(string uri)
    {
        try
        {
            var check = new Uri(uri);
            var home = new Uri(_configuration["SiteUrl"]);

            // بررسی میزبان‌هایی که نباید از طریق پروکسی ارسال شوند
            var bypassHosts = _configuration["ProxySettings:BypassHosts"]?.Split(",");
            if (bypassHosts != null)
            {
                var wildcardRegex = bypassHosts
                    .Where(host => host.Contains("*"))
                    .Select(host => "^" + Regex.Escape(host).Replace("\\*", ".+") + "$")
                    .ToArray();

                var regexPattern = "(" + string.Join("|", wildcardRegex) + ")";
                var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

                if (regex.IsMatch(check.Host))
                {
                    return false;
                }

                if (bypassHosts.Any(host => host.Equals(check.Host, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }

            // بررسی localhost و میزبان سایت
            if (check.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                check.Host.Equals(home.Host, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SendThroughProxy: {ex.Message}");
            return true; // در صورت خطا، درخواست از طریق پروکسی ارسال شود
        }
    }
}