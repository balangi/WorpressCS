using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ScriptLoaderService
{
    private readonly ILogger<ScriptLoaderService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Dictionary<string, Script> _scripts = new Dictionary<string, Script>();

    public ScriptLoaderService(ILogger<ScriptLoaderService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Adds a script to the loader.
    /// </summary>
    public void AddScript(string handle, string src, List<string> dependencies = null, string version = null, bool isInFooter = false)
    {
        if (_scripts.ContainsKey(handle))
        {
            _logger.LogWarning($"Script with handle '{handle}' already exists.");
            return;
        }

        _scripts[handle] = new Script
        {
            Handle = handle,
            Src = src,
            Dependencies = dependencies ?? new List<string>(),
            Version = version,
            IsInFooter = isInFooter
        };
    }

    /// <summary>
    /// Retrieves a script by its handle.
    /// </summary>
    public Script GetScript(string handle)
    {
        return _scripts.ContainsKey(handle) ? _scripts[handle] : null;
    }

    /// <summary>
    /// Prints all scripts in the HTML head or footer.
    /// </summary>
    public string PrintScripts(bool isFooter = false)
    {
        var scriptsToPrint = _scripts.Values.Where(s => s.IsInFooter == isFooter).ToList();
        if (!scriptsToPrint.Any())
        {
            return string.Empty;
        }

        var scriptTags = new List<string>();
        foreach (var script in scriptsToPrint)
        {
            var fullSrc = BuildScriptSrc(script);
            scriptTags.Add($"<script src=\"{fullSrc}\"></script>");
        }

        return string.Join("\n", scriptTags);
    }

    /// <summary>
    /// Builds the full URL for a script.
    /// </summary>
    private string BuildScriptSrc(Script script)
    {
        var src = script.Src;
        if (!string.IsNullOrEmpty(script.Version))
        {
            src += $"?ver={script.Version}";
        }

        // Check if the script URL is relative and prepend the base URL
        if (!Uri.IsWellFormedUriString(src, UriKind.Absolute))
        {
            var request = _httpContextAccessor.HttpContext.Request;
            src = $"{request.Scheme}://{request.Host}{src}";
        }

        return src;
    }

    /// <summary>
    /// Adds localization data to a script.
    /// </summary>
    public void LocalizeScript(string handle, string key, object value)
    {
        if (!_scripts.ContainsKey(handle))
        {
            _logger.LogError($"Script with handle '{handle}' not found.");
            return;
        }

        _scripts[handle].Localizations[key] = value;
    }

    /// <summary>
    /// Prints inline localization data for a script.
    /// </summary>
    public string PrintInlineLocalization(string handle)
    {
        if (!_scripts.ContainsKey(handle))
        {
            _logger.LogError($"Script with handle '{handle}' not found.");
            return string.Empty;
        }

        var script = _scripts[handle];
        if (script.Localizations.Count == 0)
        {
            return string.Empty;
        }

        var json = System.Text.Json.JsonSerializer.Serialize(script.Localizations);
        return $"<script>var {handle}_settings = {json};</script>";
    }
}