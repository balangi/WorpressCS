using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

public class ScriptModuleService
{
    private readonly Dictionary<string, ScriptModule> _scriptModules = new Dictionary<string, ScriptModule>();
    private readonly ILogger<ScriptModuleService> _logger;

    public ScriptModuleService(ILogger<ScriptModuleService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registers a script module if it hasn't been registered yet.
    /// </summary>
    public void RegisterScriptModule(string id, string src, List<Dependency> dependencies = null, string version = null)
    {
        if (_scriptModules.ContainsKey(id))
        {
            _logger.LogWarning($"Script module with ID '{id}' is already registered.");
            return;
        }

        var scriptModule = new ScriptModule
        {
            Id = id,
            Src = src,
            Dependencies = dependencies ?? new List<Dependency>(),
            Version = version ?? GetWordPressVersion()
        };

        _scriptModules[id] = scriptModule;
    }

    /// <summary>
    /// Marks a script module to be enqueued in the page.
    /// </summary>
    public void EnqueueScriptModule(string id, string src = null, List<Dependency> dependencies = null, string version = null)
    {
        if (!_scriptModules.ContainsKey(id))
        {
            RegisterScriptModule(id, src, dependencies, version);
        }

        _scriptModules[id].IsEnqueued = true;
    }

    /// <summary>
    /// Unmarks a script module so it is no longer enqueued in the page.
    /// </summary>
    public void DequeueScriptModule(string id)
    {
        if (_scriptModules.ContainsKey(id))
        {
            _scriptModules[id].IsEnqueued = false;
        }
    }

    /// <summary>
    /// Deregisters a script module.
    /// </summary>
    public void DeregisterScriptModule(string id)
    {
        if (_scriptModules.ContainsKey(id))
        {
            _scriptModules.Remove(id);
        }
    }

    /// <summary>
    /// Registers all default WordPress script modules.
    /// </summary>
    public void RegisterDefaultScriptModules(string basePath)
    {
        var assetsFile = Path.Combine(basePath, "assets", "script-modules-packages.php");
        if (!File.Exists(assetsFile))
        {
            _logger.LogError($"Assets file not found: {assetsFile}");
            return;
        }

        var assets = LoadAssets(assetsFile); // Simulate loading assets from a PHP file
        foreach (var asset in assets)
        {
            var fileName = asset.Key;
            var scriptModuleData = asset.Value;

            var scriptModuleId = "@wordpress/" + fileName
                .Replace("/index", "")
                .Replace(".min.js", "")
                .Replace(".js", "");

            switch (scriptModuleId)
            {
                case "@wordpress/interactivity/debug":
                    if (!IsScriptDebugMode())
                    {
                        continue;
                    }
                    scriptModuleId = "@wordpress/interactivity";
                    break;
                case "@wordpress/interactivity":
                    if (IsScriptDebugMode())
                    {
                        continue;
                    }
                    break;
            }

            var path = $"/wp-includes/js/dist/script-modules/{fileName}";
            RegisterScriptModule(scriptModuleId, path, scriptModuleData.Dependencies, scriptModuleData.Version);
        }
    }

    /// <summary>
    /// Loads assets from a PHP file (simulated).
    /// </summary>
    private Dictionary<string, ScriptModuleData> LoadAssets(string filePath)
    {
        // Simulate loading assets from a PHP file
        return new Dictionary<string, ScriptModuleData>
        {
            {
                "interactivity/index.min.js",
                new ScriptModuleData
                {
                    Dependencies = new List<Dependency>
                    {
                        new Dependency { Id = "@wordpress/dependency1", ImportType = "static" }
                    },
                    Version = "1.0.0"
                }
            }
        };
    }

    /// <summary>
    /// Gets the current WordPress version.
    /// </summary>
    private string GetWordPressVersion()
    {
        // Simulate fetching the WordPress version
        return "6.5.0";
    }

    /// <summary>
    /// Checks if script debug mode is enabled.
    /// </summary>
    private bool IsScriptDebugMode()
    {
        // Simulate checking for script debug mode
        return false;
    }
}

public class ScriptModuleData
{
    public List<Dependency> Dependencies { get; set; }
    public string Version { get; set; }
}