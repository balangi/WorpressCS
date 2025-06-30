using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class KsesService
{
    private readonly KsesSettings _settings;

    public KsesService(KsesSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Filters the content to allow only specific HTML tags and attributes.
    /// </summary>
    public string FilterContent(string content)
    {
        return FilterTags(content);
    }

    /// <summary>
    /// Filters HTML tags based on allowed tags and attributes.
    /// </summary>
    private string FilterTags(string content)
    {
        var regex = new Regex(@"<([a-zA-Z0-9]+)([^>]*)>(.*?)</\1>|<([a-zA-Z0-9]+)([^>]*)/>");
        return regex.Replace(content, match =>
        {
            var tagName = match.Groups[1].Value.ToLower();
            var attributes = match.Groups[2].Value;
            var innerContent = match.Groups[3].Value;

            if (_settings.AllowedTags.Any(tag => tag.TagName == tagName))
            {
                var allowedAttributes = _settings.AllowedTags.First(tag => tag.TagName == tagName).Attributes;
                var filteredAttributes = FilterAttributes(attributes, allowedAttributes);

                return $"<{tagName}{filteredAttributes}>{FilterTags(innerContent)}</{tagName}>";
            }

            return innerContent; // Remove disallowed tags
        });
    }

    /// <summary>
    /// Filters attributes based on allowed attributes for a tag.
    /// </summary>
    private string FilterAttributes(string attributes, Dictionary<string, bool> allowedAttributes)
    {
        var attrRegex = new Regex(@"(\w+)=(['""]?)([^'"">]*)\2");
        var filteredAttributes = new List<string>();

        foreach (Match attrMatch in attrRegex.Matches(attributes))
        {
            var attrName = attrMatch.Groups[1].Value.ToLower();
            var attrValue = attrMatch.Groups[3].Value;

            if (allowedAttributes.ContainsKey(attrName))
            {
                if (IsAttributeValueValid(attrName, attrValue))
                {
                    filteredAttributes.Add($"{attrName}=\"{attrValue}\"");
                }
            }
        }

        return string.Join(" ", filteredAttributes);
    }

    /// <summary>
    /// Validates attribute values based on allowed protocols.
    /// </summary>
    private bool IsAttributeValueValid(string attributeName, string attributeValue)
    {
        if (attributeName == "href" || attributeName == "src")
        {
            var uri = new Uri(attributeValue, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
            {
                return _settings.AllowedProtocols.Contains(uri.Scheme);
            }
        }

        return true;
    }

    /// <summary>
    /// Normalizes HTML entities in the content.
    /// </summary>
    public string NormalizeEntities(string content)
    {
        foreach (var entity in _settings.AllowedEntities)
        {
            content = content.Replace($"&{entity};", $"&{entity};");
        }

        return content;
    }

    /// <summary>
    /// Decodes numeric HTML entities.
    /// </summary>
    public string DecodeEntities(string content)
    {
        content = Regex.Replace(content, @"&#([0-9]+);", match => ((char)int.Parse(match.Groups[1].Value)).ToString());
        content = Regex.Replace(content, @"&#x([0-9A-Fa-f]+);", match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString());
        return content;
    }
}