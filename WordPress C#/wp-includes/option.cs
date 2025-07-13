using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class OptionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<OptionService> _logger;

    public OptionService(ApplicationDbContext context, IMemoryCache cache, ILogger<OptionService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves an option value based on the option name.
    /// </summary>
    public object GetOption(string optionName, object defaultValue = null)
    {
        if (string.IsNullOrEmpty(optionName))
        {
            throw new ArgumentNullException(nameof(optionName));
        }

        var cachedOptions = _cache.Get<Dictionary<string, object>>("options");
        if (cachedOptions != null && cachedOptions.ContainsKey(optionName))
        {
            return cachedOptions[optionName];
        }

        var notOptions = _cache.Get<HashSet<string>>("notoptions") ?? new HashSet<string>();
        if (notOptions.Contains(optionName))
        {
            return defaultValue;
        }

        var option = _context.Options.FirstOrDefault(o => o.OptionName == optionName);
        if (option == null)
        {
            notOptions.Add(optionName);
            _cache.Set("notoptions", notOptions);
            return defaultValue;
        }

        var value = DeserializeOptionValue(option.OptionValue);
        CacheOption(optionName, value);

        return value;
    }

    /// <summary>
    /// Updates or adds a new option.
    /// </summary>
    public bool UpdateOption(string optionName, object value, bool autoload = true)
    {
        if (string.IsNullOrEmpty(optionName))
        {
            throw new ArgumentNullException(nameof(optionName));
        }

        ProtectSpecialOption(optionName);

        var serializedValue = SerializeOptionValue(value);
        var existingOption = _context.Options.FirstOrDefault(o => o.OptionName == optionName);

        if (existingOption != null)
        {
            existingOption.OptionValue = serializedValue;
            existingOption.Autoload = autoload;
        }
        else
        {
            _context.Options.Add(new Option
            {
                OptionName = optionName,
                OptionValue = serializedValue,
                Autoload = autoload
            });
        }

        _context.SaveChanges();
        CacheOption(optionName, value);

        return true;
    }

    /// <summary>
    /// Deletes an option.
    /// </summary>
    public bool DeleteOption(string optionName)
    {
        if (string.IsNullOrEmpty(optionName))
        {
            throw new ArgumentNullException(nameof(optionName));
        }

        ProtectSpecialOption(optionName);

        var option = _context.Options.FirstOrDefault(o => o.OptionName == optionName);
        if (option == null)
        {
            return false;
        }

        _context.Options.Remove(option);
        _context.SaveChanges();

        RemoveFromCache(optionName);

        return true;
    }

    /// <summary>
    /// Primes the cache for all autoload options.
    /// </summary>
    public void PrimeAutoloadOptions()
    {
        var autoloadOptions = _context.Options.Where(o => o.Autoload).ToList();
        var optionsDict = autoloadOptions.ToDictionary(o => o.OptionName, o => DeserializeOptionValue(o.OptionValue));

        _cache.Set("options", optionsDict);
    }

    /// <summary>
    /// Serializes an option value.
    /// </summary>
    private string SerializeOptionValue(object value)
    {
        return System.Text.Json.JsonSerializer.Serialize(value);
    }

    /// <summary>
    /// Deserializes an option value.
    /// </summary>
    private object DeserializeOptionValue(string value)
    {
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<object>(value);
        }
        catch
        {
            return value; // Return as-is if deserialization fails
        }
    }

    /// <summary>
    /// Caches an option value.
    /// </summary>
    private void CacheOption(string optionName, object value)
    {
        var cachedOptions = _cache.Get<Dictionary<string, object>>("options") ?? new Dictionary<string, object>();
        cachedOptions[optionName] = value;
        _cache.Set("options", cachedOptions);
    }

    /// <summary>
    /// Removes an option from the cache.
    /// </summary>
    private void RemoveFromCache(string optionName)
    {
        var cachedOptions = _cache.Get<Dictionary<string, object>>("options");
        if (cachedOptions != null && cachedOptions.ContainsKey(optionName))
        {
            cachedOptions.Remove(optionName);
            _cache.Set("options", cachedOptions);
        }

        var notOptions = _cache.Get<HashSet<string>>("notoptions");
        if (notOptions != null && notOptions.Contains(optionName))
        {
            notOptions.Remove(optionName);
            _cache.Set("notoptions", notOptions);
        }
    }

    /// <summary>
    /// Protects special options from being modified.
    /// </summary>
    private void ProtectSpecialOption(string optionName)
    {
        if (optionName == "alloptions" || optionName == "notoptions")
        {
            throw new InvalidOperationException($"'{optionName}' is a protected option and cannot be modified.");
        }
    }
}