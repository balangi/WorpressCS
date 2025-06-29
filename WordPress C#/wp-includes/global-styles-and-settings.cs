using System;
using System.Collections.Generic;
using System.Linq;

public class GlobalStylesAndSettingsService
{
    private readonly ApplicationDbContext _context;

    public GlobalStylesAndSettingsService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets global settings based on the provided path and context.
    /// </summary>
    public object GetGlobalSettings(List<string> path = null, Dictionary<string, object> context = null)
    {
        var cacheKey = GenerateCacheKey("settings", path, context);
        var cachedData = GetCachedData(cacheKey);

        if (cachedData != null)
        {
            return cachedData.Data;
        }

        var mergedData = MergeThemeJsonData();
        var settings = ExtractSettings(mergedData, path, context);

        CacheData(cacheKey, settings);
        return settings;
    }

    /// <summary>
    /// Gets global styles based on the provided path and context.
    /// </summary>
    public object GetGlobalStyles(List<string> path = null, Dictionary<string, object> context = null)
    {
        var cacheKey = GenerateCacheKey("styles", path, context);
        var cachedData = GetCachedData(cacheKey);

        if (cachedData != null)
        {
            return cachedData.Data;
        }

        var mergedData = MergeThemeJsonData();
        var styles = ExtractStyles(mergedData, path, context);

        CacheData(cacheKey, styles);
        return styles;
    }

    /// <summary>
    /// Generates a global stylesheet based on the provided types.
    /// </summary>
    public string GenerateGlobalStylesheet(List<string> types = null)
    {
        var cacheKey = GenerateCacheKey("stylesheet", types);
        var cachedData = GetCachedData(cacheKey);

        if (cachedData != null)
        {
            return cachedData.Data.ToString();
        }

        var mergedData = MergeThemeJsonData();
        var stylesheet = BuildStylesheet(mergedData, types);

        CacheData(cacheKey, stylesheet);
        return stylesheet;
    }

    /// <summary>
    /// Merges theme.json data from core, theme, and user sources.
    /// </summary>
    private ThemeJsonData MergeThemeJsonData()
    {
        // Logic to merge data from core, theme, and user sources
        return new ThemeJsonData
        {
            Settings = new Dictionary<string, object>
            {
                { "color", new { background = "#ffffff", text = "#000000" } },
                { "typography", new { fontSize = "16px" } }
            },
            Styles = new Dictionary<string, object>
            {
                { "global", new { color = "#ffffff", fontSize = "16px" } },
                { "blocks", new Dictionary<string, object>
                    {
                        { "core/paragraph", new { color = "#000000", fontSize = "14px" } }
                    }
                }
            }
        };
    }

    /// <summary>
    /// Extracts settings from the merged theme.json data.
    /// </summary>
    private object ExtractSettings(ThemeJsonData mergedData, List<string> path, Dictionary<string, object> context)
    {
        var settings = mergedData.Settings;

        if (path != null && path.Any())
        {
            foreach (var key in path)
            {
                if (settings.ContainsKey(key))
                {
                    settings = new Dictionary<string, object> { { key, settings[key] } };
                }
            }
        }

        return settings;
    }

    /// <summary>
    /// Extracts styles from the merged theme.json data.
    /// </summary>
    private object ExtractStyles(ThemeJsonData mergedData, List<string> path, Dictionary<string, object> context)
    {
        var styles = mergedData.Styles;

        if (path != null && path.Any())
        {
            foreach (var key in path)
            {
                if (styles.ContainsKey(key))
                {
                    styles = new Dictionary<string, object> { { key, styles[key] } };
                }
            }
        }

        return styles;
    }

    /// <summary>
    /// Builds a stylesheet from the merged theme.json data.
    /// </summary>
    private string BuildStylesheet(ThemeJsonData mergedData, List<string> types)
    {
        var styles = mergedData.Styles;
        var stylesheet = new StringBuilder();

        foreach (var style in styles)
        {
            stylesheet.AppendLine($"{style.Key} {{");
            if (style.Value is Dictionary<string, object> styleProperties)
            {
                foreach (var property in styleProperties)
                {
                    stylesheet.AppendLine($"  {property.Key}: {property.Value};");
                }
            }
            stylesheet.AppendLine("}");
        }

        return stylesheet.ToString();
    }

    /// <summary>
    /// Generates a cache key based on the provided parameters.
    /// </summary>
    private string GenerateCacheKey(string type, List<string> path = null, Dictionary<string, object> context = null)
    {
        var key = type;
        if (path != null && path.Any())
        {
            key += "_" + string.Join("_", path);
        }
        if (context != null && context.Any())
        {
            key += "_" + string.Join("_",