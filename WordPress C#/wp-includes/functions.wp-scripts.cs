using System;
using System.Collections.Generic;
using System.Linq;

public class ScriptService
{
    private readonly ScriptRegistry _scriptRegistry;

    public ScriptService()
    {
        _scriptRegistry = new ScriptRegistry();
    }

    /// <summary>
    /// Initializes the script registry if it has not been set.
    /// </summary>
    public void InitializeScripts()
    {
        if (_scriptRegistry.Scripts == null)
        {
            _scriptRegistry.Scripts = new Dictionary<string, Script>();
        }
    }

    /// <summary>
    /// Registers a new script.
    /// </summary>
    public void RegisterScript(string handle, string src, List<string> dependencies = null, string version = null, bool inFooter = false, string strategy = null)
    {
        if (!_scriptRegistry.Scripts.ContainsKey(handle))
        {
            _scriptRegistry.Scripts[handle] = new Script
            {
                Handle = handle,
                Src = src,
                Dependencies = dependencies ?? new List<string>(),
                Version = version,
                InFooter = inFooter,
                Strategy = strategy
            };
        }
    }

    /// <summary>
    /// Enqueues a script for loading.
    /// </summary>
    public void EnqueueScript(string handle)
    {
        if (_scriptRegistry.Scripts.ContainsKey(handle))
        {
            var script = _scriptRegistry.Scripts[handle];
            // Logic to enqueue the script (e.g., add to a queue list).
            Console.WriteLine($"Enqueued script: {handle}");
        }
        else
        {
            throw new ArgumentException($"Script with handle '{handle}' is not registered.");
        }
    }

    /// <summary>
    /// Removes a registered script.
    /// </summary>
    public void DeregisterScript(string handle)
    {
        if (_scriptRegistry.Scripts.ContainsKey(handle))
        {
            _scriptRegistry.Scripts.Remove(handle);
            Console.WriteLine($"Deregistered script: {handle}");
        }
        else
        {
            throw new ArgumentException($"Script with handle '{handle}' is not registered.");
        }
    }

    /// <summary>
    /// Adds inline script data to a registered script.
    /// </summary>
    public void AddInlineScript(string handle, string data, string position = "after")
    {
        if (_scriptRegistry.Scripts.ContainsKey(handle))
        {
            var script = _scriptRegistry.Scripts[handle];
            script.InlineScripts[position] = data;
            Console.WriteLine($"Added inline script to handle '{handle}' at position '{position}'.");
        }
        else
        {
            throw new ArgumentException($"Script with handle '{handle}' is not registered.");
        }
    }

    /// <summary>
    /// Localizes a script with data.
    /// </summary>
    public void LocalizeScript(string handle, string objectName, Dictionary<string, object> localizationData)
    {
        if (_scriptRegistry.Scripts.ContainsKey(handle))
        {
            var script = _scriptRegistry.Scripts[handle];
            script.Localizations[objectName] = localizationData;
            Console.WriteLine($"Localized script: {handle} with object name: {objectName}");
        }
        else
        {
            throw new ArgumentException($"Script with handle '{handle}' is not registered.");
        }
    }

    /// <summary>
    /// Checks if a script is registered.
    /// </summary>
    public bool IsScriptRegistered(string handle)
    {
        return _scriptRegistry.Scripts.ContainsKey(handle);
    }

    /// <summary>
    /// Prints all registered scripts.
    /// </summary>
    public void PrintScripts()
    {
        foreach (var script in _scriptRegistry.Scripts.Values)
        {
            Console.WriteLine($"Handle: {script.Handle}, Src: {script.Src}, Dependencies: {string.Join(", ", script.Dependencies)}");
        }
    }
}