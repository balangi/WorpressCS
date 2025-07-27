using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

public class RobotsService
{
    private readonly ILogger<RobotsService> _logger;

    public RobotsService(ILogger<RobotsService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the robots meta tag as necessary.
    /// </summary>
    public string RenderRobotsMetaTag(Dictionary<string, string> robotsDirectives)
    {
        if (robotsDirectives == null || robotsDirectives.Count == 0)
        {
            return string.Empty;
        }

        var robotsStrings = new List<string>();
        foreach (var directive in robotsDirectives)
        {
            if (!string.IsNullOrEmpty(directive.Value))
            {
                robotsStrings.Add($"{directive.Key}:{directive.Value}");
            }
            else
            {
                robotsStrings.Add(directive.Key);
            }
        }

        if (robotsStrings.Count == 0)
        {
            return string.Empty;
        }

        return $"<meta name='robots' content='{string.Join(", ", robotsStrings)}' />\n";
    }

    /// <summary>
    /// Adds 'noindex' to the robots meta tag if required by the site configuration.
    /// </summary>
    public Dictionary<string, string> AddNoIndexIfRequired(Dictionary<string, string> robotsDirectives, bool isPublic)
    {
        if (!isPublic)
        {
            robotsDirectives["noindex"] = "true";
            robotsDirectives["nofollow"] = "true";
        }

        return robotsDirectives;
    }

    /// <summary>
    /// Adds 'noindex' to the robots meta tag for embeds.
    /// </summary>
    public Dictionary<string, string> AddNoIndexForEmbeds(Dictionary<string, string> robotsDirectives, bool isEmbed)
    {
        if (isEmbed)
        {
            robotsDirectives["noindex"] = "true";
        }

        return robotsDirectives;
    }

    /// <summary>
    /// Adds 'noindex' to the robots meta tag for search pages.
    /// </summary>
    public Dictionary<string, string> AddNoIndexForSearch(Dictionary<string, string> robotsDirectives, bool isSearch)
    {
        if (isSearch)
        {
            robotsDirectives["noindex"] = "true";
        }

        return robotsDirectives;
    }

    /// <summary>
    /// Adds 'noindex' and 'noarchive' to the robots meta tag for sensitive pages.
    /// </summary>
    public Dictionary<string, string> AddSensitivePageDirectives(Dictionary<string, string> robotsDirectives)
    {
        robotsDirectives["noindex"] = "true";
        robotsDirectives["noarchive"] = "true";

        return robotsDirectives;
    }

    /// <summary>
    /// Adds 'max-image-preview:large' to the robots meta tag.
    /// </summary>
    public Dictionary<string, string> AddMaxImagePreviewLarge(Dictionary<string, string> robotsDirectives, bool isPublic)
    {
        if (isPublic)
        {
            robotsDirectives["max-image-preview"] = "large";
        }

        return robotsDirectives;
    }
}