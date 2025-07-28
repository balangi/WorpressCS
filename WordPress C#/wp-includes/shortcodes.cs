using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

public class ShortcodeService
{
    private readonly Dictionary<string, Func<Dictionary<string, string>, string, string>> _shortcodeTags = new Dictionary<string, Func<Dictionary<string, string>, string, string>>();
    private readonly ILogger<ShortcodeService> _logger;

    public ShortcodeService(ILogger<ShortcodeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Adds a new shortcode.
    /// </summary>
    public void AddShortcode(string tag, Func<Dictionary<string, string>, string, string> callback)
    {
        if (_shortcodeTags.ContainsKey(tag))
        {
            _logger.LogWarning($"Shortcode with tag '{tag}' is already registered.");
            return;
        }

        _shortcodeTags[tag] = callback;
    }

    /// <summary>
    /// Removes a shortcode.
    /// </summary>
    public void RemoveShortcode(string tag)
    {
        if (!_shortcodeTags.ContainsKey(tag))
        {
            _logger.LogWarning($"Shortcode with tag '{tag}' does not exist.");
            return;
        }

        _shortcodeTags.Remove(tag);
    }

    /// <summary>
    /// Clears all shortcodes.
    /// </summary>
    public void RemoveAllShortcodes()
    {
        _shortcodeTags.Clear();
    }

    /// <summary>
    /// Determines whether a shortcode exists.
    /// </summary>
    public bool ShortcodeExists(string tag)
    {
        return _shortcodeTags.ContainsKey(tag);
    }

    /// <summary>
    /// Processes content to replace shortcodes with their output.
    /// </summary>
    public string ProcessShortcodes(string content, bool ignoreHtml = false)
    {
        if (string.IsNullOrEmpty(content) || !_shortcodeTags.Any())
        {
            return content;
        }

        var regex = GetShortcodeRegex();
        return Regex.Replace(content, regex, match =>
        {
            var tag = match.Groups[2].Value;
            if (!_shortcodeTags.ContainsKey(tag))
            {
                _logger.LogWarning($"Shortcode with tag '{tag}' does not exist.");
                return match.Value;
            }

            var attributes = ParseAttributes(match.Groups[3].Value);
            var innerContent = match.Groups[5].Value;

            try
            {
                return _shortcodeTags[tag](attributes, innerContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing shortcode with tag '{tag}'.");
                return match.Value;
            }
        });
    }

    /// <summary>
    /// Retrieves the regular expression for matching shortcodes.
    /// </summary>
    private string GetShortcodeRegex()
    {
        var tags = string.Join("|", _shortcodeTags.Keys.Select(Regex.Escape));
        return $@"\[(\[?)({tags})(?![\w-])([^\]]*)?(\]?)(?:([^\[]*?)\[\/\2\])?(\]?)";
    }

    /// <summary>
    /// Parses shortcode attributes into a dictionary.
    /// </summary>
    private Dictionary<string, string> ParseAttributes(string attributesString)
    {
        var attributes = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(attributesString))
        {
            return attributes;
        }

        var matches = Regex.Matches(attributesString, @"([\w-]+)\s*=\s*""([^""]*)""|([\w-]+)\s*=\s*'([^']*)'|([\w-]+)\s*=\s*([^\s'""/>]*)|""([^""]*)""|'([^']*)'|(\S+)");
        foreach (Match match in matches)
        {
            var key = match.Groups[1].Value ?? match.Groups[3].Value ?? match.Groups[5].Value;
            var value = match.Groups[2].Value ?? match.Groups[4].Value ?? match.Groups[6].Value;
            if (!string.IsNullOrEmpty(key))
            {
                attributes[key] = value;
            }
        }

        return attributes;
    }
}