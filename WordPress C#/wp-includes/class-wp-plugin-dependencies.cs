using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class PluginDependenciesService
{
    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<PluginDependenciesService> _logger;

    /// <summary>
    /// HttpClient برای درخواست‌های HTTP
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// لیست وابستگی‌ها
    /// </summary>
    private Dictionary<string, List<string>> _dependencies = new();

    /// <summary>
    /// لیست پلاگین‌ها
    /// </summary>
    private Dictionary<string, PluginInfo> _plugins = new();

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public PluginDependenciesService(IMemoryCache cache, ILogger<PluginDependenciesService> logger, HttpClient httpClient)
    {
        _cache = cache;
        _logger = logger;
        _httpClient = httpClient;
    }

    /// <summary>
    /// خواندن وابستگی‌ها از هدرهای پلاگین‌ها
    /// </summary>
    public void ReadDependenciesFromPluginHeaders(Dictionary<string, PluginInfo> plugins)
    {
        _dependencies.Clear();
        foreach (var plugin in plugins)
        {
            var requiresPlugins = plugin.Value.RequiresPlugins;
            if (!string.IsNullOrEmpty(requiresPlugins))
            {
                var dependencies = requiresPlugins.Split(',')
                                                  .Select(slug => slug.Trim())
                                                  .ToList();
                _dependencies[plugin.Key] = dependencies;
            }
        }
    }

    /// <summary>
    /// بررسی وجود وابستگی‌های نصب‌نشده یا غیرفعال
    /// </summary>
    public bool HasUnmetDependencies(string pluginFile)
    {
        if (!_dependencies.ContainsKey(pluginFile))
        {
            return false;
        }

        foreach (var dependency in _dependencies[pluginFile])
        {
            if (!_plugins.ContainsKey(dependency) || !_plugins[dependency].IsActive)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// دریافت اطلاعات وابستگی‌ها از API WordPress
    /// </summary>
    public async Task<Dictionary<string, PluginApiData>> GetDependencyApiDataAsync(IEnumerable<string> slugs)
    {
        var apiData = new Dictionary<string, PluginApiData>();

        foreach (var slug in slugs)
        {
            if (_cache.TryGetValue($"plugin_api_{slug}", out PluginApiData cachedData))
            {
                apiData[slug] = cachedData;
                continue;
            }

            try
            {
                var response = await _httpClient.GetAsync($"https://api.wordpress.org/plugins/info/1.0/{slug}.json");
                if (response.IsSuccessStatusCode)
                {
                    var data = await JsonSerializer.DeserializeAsync<PluginApiData>(await response.Content.ReadAsStreamAsync());
                    apiData[slug] = data;
                    _cache.Set($"plugin_api_{slug}", data, TimeSpan.FromHours(12));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching API data for plugin: {Slug}", slug);
            }
        }

        return apiData;
    }

    /// <summary>
    /// بررسی وجود وابستگی‌های چرخشی
    /// </summary>
    public bool HasCircularDependency(string pluginFile)
    {
        var visited = new HashSet<string>();
        return CheckCircularDependency(pluginFile, visited);
    }

    private bool CheckCircularDependency(string pluginFile, HashSet<string> visited)
    {
        if (visited.Contains(pluginFile))
        {
            return true;
        }

        visited.Add(pluginFile);

        if (_dependencies.ContainsKey(pluginFile))
        {
            foreach (var dependency in _dependencies[pluginFile])
            {
                if (CheckCircularDependency(dependency, visited))
                {
                    return true;
                }
            }
        }

        visited.Remove(pluginFile);
        return false;
    }
}

/// <summary>
/// اطلاعات پلاگین
/// </summary>
public class PluginInfo
{
    public string Name { get; set; }
    public string RequiresPlugins { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// اطلاعات API پلاگین
/// </summary>
public class PluginApiData
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string LastUpdated { get; set; }
}