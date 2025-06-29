using System;
using System.Collections.Generic;
using System.Linq;

public class StyleService
{
    private readonly StyleRegistry _styleRegistry;

    public StyleService()
    {
        _styleRegistry = new StyleRegistry();
    }

    /// <summary>
    /// Initializes the style registry if it has not been set.
    /// </summary>
    public void InitializeStyles()
    {
        if (_styleRegistry.Styles == null)
        {
            _styleRegistry.Styles = new Dictionary<string, Style>();
        }
    }

    /// <summary>
    /// Registers a new style.
    /// </summary>
    public bool RegisterStyle(string handle, string src, List<string> dependencies = null, string version = null, string media = "all")
    {
        if (!_styleRegistry.Styles.ContainsKey(handle))
        {
            _styleRegistry.Styles[handle] = new Style
            {
                Handle = handle,
                Src = src,
                Dependencies = dependencies ?? new List<string>(),
                Version = version,
                Media = media
            };
            return true;
        }
        return false;
    }

    /// <summary>
    /// Enqueues a style for loading.
    /// </summary>
    public void EnqueueStyle(string handle, string src = "", List<string> dependencies = null, string version = null, string media = "all")
    {
        if (string.IsNullOrEmpty(src))
        {
            if (!_styleRegistry.Styles.ContainsKey(handle))
            {
                throw new ArgumentException($"Style with handle '{handle}' is not registered.");
            }
        }
        else
        {
            RegisterStyle(handle, src, dependencies, version, media);
        }

        Console.WriteLine($"Enqueued style: {handle}");
    }

    /// <summary>
    /// Removes a registered style.
    /// </summary>
    public void DeregisterStyle(string handle)
    {
        if (_styleRegistry.Styles.ContainsKey(handle))
        {
            _styleRegistry.Styles.Remove(handle);
            Console.WriteLine($"Deregistered style: {handle}");
        }
        else
        {
            throw new ArgumentException($"Style with handle '{handle}' is not registered.");
        }
    }

    /// <summary>
    /// Adds inline CSS to a registered style.
    /// </summary>
    public bool AddInlineStyle(string handle, string data)
    {
        if (_styleRegistry.Styles.ContainsKey(handle))
        {
            var style = _styleRegistry.Styles[handle];
            style.InlineStyles.Add(data);
            Console.WriteLine($"Added inline style to handle '{handle}'.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds metadata to a registered style.
    /// </summary>
    public bool AddStyleMetadata(string handle, string key, object value)
    {
        if (_styleRegistry.Styles.ContainsKey(handle))
        {
            var style = _styleRegistry.Styles[handle];
            style.Metadata[key] = value;
            Console.WriteLine($"Added metadata '{key}' to handle '{handle}'.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a style is registered.
    /// </summary>
    public bool IsStyleRegistered(string handle)
    {
        return _styleRegistry.Styles.ContainsKey(handle);
    }

    /// <summary>
    /// Prints all registered styles.
    /// </summary>
    public void PrintStyles()
    {
        foreach (var style in _styleRegistry.Styles.Values)
        {
            Console.WriteLine($"Handle: {style.Handle}, Src: {style.Src}, Dependencies: {string.Join(", ", style.Dependencies)}");
        }
    }
}