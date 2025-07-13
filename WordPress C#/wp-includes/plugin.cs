using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

public class PluginService
{
    private readonly Dictionary<string, Hook> _hooks = new Dictionary<string, Hook>();
    private readonly Dictionary<string, Filter> _filters = new Dictionary<string, Filter>();
    private readonly ILogger<PluginService> _logger;

    public PluginService(ILogger<PluginService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Adds an action hook.
    /// </summary>
    public void AddAction(string hookName, Delegate callback, int priority = 10)
    {
        if (string.IsNullOrEmpty(hookName))
        {
            throw new ArgumentNullException(nameof(hookName));
        }

        if (!_hooks.ContainsKey(hookName))
        {
            _hooks[hookName] = new Hook { Name = hookName };
        }

        var hook = _hooks[hookName];
        hook.Callbacks.Add(new HookCallback
        {
            Id = Guid.NewGuid().ToString(),
            Callback = callback,
            Priority = priority
        });

        // Sort callbacks by priority
        hook.Callbacks = hook.Callbacks.OrderBy(cb => cb.Priority).ToList();
    }

    /// <summary>
    /// Executes all callbacks for a given action hook.
    /// </summary>
    public void DoAction(string hookName, params object[] args)
    {
        if (!_hooks.ContainsKey(hookName))
        {
            _logger.LogWarning($"Hook '{hookName}' not found.");
            return;
        }

        var hook = _hooks[hookName];
        foreach (var callback in hook.Callbacks)
        {
            try
            {
                callback.Callback.DynamicInvoke(args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing callback for hook '{hookName}'.");
            }
        }
    }

    /// <summary>
    /// Adds a filter hook.
    /// </summary>
    public void AddFilter(string filterName, Delegate callback, int priority = 10)
    {
        if (string.IsNullOrEmpty(filterName))
        {
            throw new ArgumentNullException(nameof(filterName));
        }

        if (!_filters.ContainsKey(filterName))
        {
            _filters[filterName] = new Filter { Name = filterName };
        }

        var filter = _filters[filterName];
        filter.Callbacks.Add(new HookCallback
        {
            Id = Guid.NewGuid().ToString(),
            Callback = callback,
            Priority = priority
        });

        // Sort callbacks by priority
        filter.Callbacks = filter.Callbacks.OrderBy(cb => cb.Priority).ToList();
    }

    /// <summary>
    /// Applies all filters for a given filter name to the input value.
    /// </summary>
    public T ApplyFilters<T>(string filterName, T value)
    {
        if (!_filters.ContainsKey(filterName))
        {
            _logger.LogWarning($"Filter '{filterName}' not found.");
            return value;
        }

        var filter = _filters[filterName];
        foreach (var callback in filter.Callbacks)
        {
            try
            {
                value = (T)callback.Callback.DynamicInvoke(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error applying filter '{filterName}'.");
            }
        }

        return value;
    }

    /// <summary>
    /// Removes a specific callback from an action or filter.
    /// </summary>
    public void RemoveCallback(string hookOrFilterName, string callbackId)
    {
        if (_hooks.ContainsKey(hookOrFilterName))
        {
            var hook = _hooks[hookOrFilterName];
            hook.Callbacks.RemoveAll(cb => cb.Id == callbackId);
        }
        else if (_filters.ContainsKey(hookOrFilterName))
        {
            var filter = _filters[hookOrFilterName];
            filter.Callbacks.RemoveAll(cb => cb.Id == callbackId);
        }
        else
        {
            _logger.LogWarning($"Hook or filter '{hookOrFilterName}' not found.");
        }
    }
}