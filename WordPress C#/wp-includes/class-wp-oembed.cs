using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

public class OEmbed
{
    /// <summary>
    /// لیست Providers
    /// </summary>
    private readonly Dictionary<string, string> _providers = new();

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<OEmbed> _logger;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public OEmbed(ILogger<OEmbed> logger)
    {
        _logger = logger;

        // اضافه کردن Providers پیش‌فرض
        AddProvider("https://*.dailymotion.com/*", "https://www.dailymotion.com/services/oembed");
        AddProvider("https://*.flickr.com/*", "https://www.flickr.com/services/oembed");
        AddProvider("https://*.vimeo.com/*", "https://vimeo.com/api/oembed.json");
        AddProvider("https://*.youtube.com/*", "https://www.youtube.com/oembed");
    }

    /// <summary>
    /// اضافه کردن Provider
    /// </summary>
    public void AddProvider(string format, string providerUrl)
    {
        _providers[format] = providerUrl;
    }

    /// <summary>
    /// حذف Provider
    /// </summary>
    public void RemoveProvider(string format)
    {
        if (_providers.ContainsKey(format))
        {
            _providers.Remove(format);
        }
    }

    /// <summary>
    /// دریافت Provider مناسب برای URL
    /// </summary>
    public string GetProvider(string url)
    {
        foreach (var provider in _providers)
        {
            var regex = new Regex(provider.Key.Replace("*", ".*"), RegexOptions.IgnoreCase);
            if (regex.IsMatch(url))
            {
                return provider.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// دریافت داده‌های oEmbed
    /// </summary>
    public async Task<object> FetchAsync(string url, Dictionary<string, string> args = null)
    {
        var providerUrl = GetProvider(url);
        if (string.IsNullOrEmpty(providerUrl))
        {
            _logger.LogWarning("No provider found for URL: {Url}", url);
            return null;
        }

        try
        {
            using var httpClient = new HttpClient();
            var requestUrl = $"{providerUrl}?url={Uri.EscapeDataString(url)}&format=json";
            var response = await httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch oEmbed data from provider: {ProviderUrl}", providerUrl);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching oEmbed data for URL: {Url}", url);
            return null;
        }
    }

    /// <summary>
    /// تبدیل داده‌های oEmbed به HTML
    /// </summary>
    public string DataToHtml(object data, string url)
    {
        if (data == null || !(data is JsonElement jsonData))
        {
            return null;
        }

        try
        {
            var type = jsonData.GetProperty("type").GetString();
            switch (type)
            {
                case "photo":
                    var photoUrl = jsonData.GetProperty("url").GetString();
                    var width = jsonData.GetProperty("width").GetInt32();
                    var height = jsonData.GetProperty("height").GetInt32();
                    return $"<img src=\"{photoUrl}\" width=\"{width}\" height=\"{height}\" alt=\"Embedded Photo\" />";

                case "video":
                    var html = jsonData.GetProperty("html").GetString();
                    return html;

                default:
                    return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting oEmbed data to HTML for URL: {Url}", url);
            return null;
        }
    }
}