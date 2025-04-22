using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class BlockTemplateUtils
{
    // Constants for supported template part areas
    public const string WP_TEMPLATE_PART_AREA_HEADER = "header";
    public const string WP_TEMPLATE_PART_AREA_FOOTER = "footer";
    public const string WP_TEMPLATE_PART_AREA_SIDEBAR = "sidebar";
    public const string WP_TEMPLATE_PART_AREA_UNCATEGORIZED = "uncategorized";

    /// <summary>
    /// Gets block theme folders for a given theme stylesheet
    /// </summary>
    public static Dictionary<string, string> GetBlockThemeFolders(string themeStylesheet = null)
    {
        var theme = ThemeHelper.GetTheme(themeStylesheet);
        if (!theme.Exists)
        {
            return new Dictionary<string, string>
            {
                { "wp_template", "templates" },
                { "wp_template_part", "parts" }
            };
        }
        return theme.GetBlockTemplateFolders();
    }

    /// <summary>
    /// Returns a filtered list of allowed area values for template parts
    /// </summary>
    public static List<Dictionary<string, object>> GetAllowedBlockTemplatePartAreas()
    {
        var defaultAreaDefinitions = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "area", WP_TEMPLATE_PART_AREA_UNCATEGORIZED },
                { "label", I18n.T("General", "template part area") },
                { "description", I18n.T("General templates often perform a specific role like displaying post content, and are not tied to any particular area.") },
                { "icon", "layout" },
                { "area_tag", "div" }
            },
            new Dictionary<string, object>
            {
                { "area", WP_TEMPLATE_PART_AREA_HEADER },
                { "label", I18n.T("Header", "template part area") },
                { "description", I18n.T("The Header template defines a page area that typically contains a title, logo, and main navigation.") },
                { "icon", "header" },
                { "area_tag", "header" }
            },
            new Dictionary<string, object>
            {
                { "area", WP_TEMPLATE_PART_AREA_FOOTER },
                { "label", I18n.T("Footer", "template part area") },
                { "description", I18n.T("The Footer template defines a page area that typically contains site credits, social links, or any other combination of blocks.") },
                { "icon", "footer" },
                { "area_tag", "footer" }
            }
        };

        // Apply filter hook equivalent to WordPress
        return FilterHooks.ApplyFilter("default_wp_template_part_areas", defaultAreaDefinitions);
    }

    /// <summary>
    /// Returns a filtered list of default template types
    /// </summary>
    public static Dictionary<string, Dictionary<string, string>> GetDefaultBlockTemplateTypes()
    {
        var defaultTemplateTypes = new Dictionary<string, Dictionary<string, string>>
        {
            { "index", new Dictionary<string, string>
                {
                    { "title", I18n.T("Index", "Template name") },
                    { "description", I18n.T("Used as a fallback template for all pages when a more specific template is not defined.") }
                }
            },
            { "home", new Dictionary<string, string>
                {
                    { "title", I18n.T("Blog Home", "Template name") },
                    { "description", I18n.T("Displays the latest posts as either the site homepage or as the \"Posts page\" as defined under reading settings. If it exists, the Front Page template overrides this template when posts are shown on the homepage.") }
                }
            },
            // ... (other template types would be added here)
        };

        // Add post format templates
        var postFormats = PostHelper.GetPostFormatStrings();
        foreach (var postFormat in postFormats)
        {
            defaultTemplateTypes[$"taxonomy-post_format-post-format-{postFormat.Key}"] = new Dictionary<string, string>
            {
                { "title", string.Format(I18n.T("Post Format: {0}", "Template name"), postFormat.Value) },
                { "description", string.Format(I18n.T("Displays the {0} post format archive."), postFormat.Value) }
            };
        }

        return FilterHooks.ApplyFilter("default_template_types", defaultTemplateTypes);
    }

    /// <summary>
    /// Checks if the input area is a supported value
    /// </summary>
    internal static string FilterBlockTemplatePartArea(string type)
    {
        var allowedAreas = GetAllowedBlockTemplatePartAreas().Select(x => x["area"].ToString()).ToList();
        if (allowedAreas.Contains(type))
        {
            return type;
        }

        var warningMessage = string.Format(
            I18n.T("\"{0}\" is not a supported wp_template_part area value and has been added as \"{1}\"."),
            type,
            WP_TEMPLATE_PART_AREA_UNCATEGORIZED
        );
        DebugHelper.TriggerError(warningMessage);
        return WP_TEMPLATE_PART_AREA_UNCATEGORIZED;
    }

    /// <summary>
    /// Finds all nested template part file paths in a theme's directory
    /// </summary>
    internal static List<string> GetBlockTemplatesPaths(string baseDirectory)
    {
        // Implementation would use Directory.EnumerateFiles with search pattern
        // Similar to the PHP RecursiveIteratorIterator approach
        var pathList = new List<string>();
        
        if (Directory.Exists(baseDirectory))
        {
            var htmlFiles = Directory.EnumerateFiles(baseDirectory, "*.html", SearchOption.AllDirectories);
            pathList.AddRange(htmlFiles);
        }
        
        return pathList;
    }

    /// <summary>
    /// Retrieves the template file from the theme for a given slug
    /// </summary>
    internal static Dictionary<string, object> GetBlockTemplateFile(string templateType, string slug)
    {
        if (templateType != "wp_template" && templateType != "wp_template_part")
        {
            return null;
        }

        var themes = new Dictionary<string, string>
        {
            { ThemeHelper.GetStylesheet(), ThemeHelper.GetStylesheetDirectory() },
            { ThemeHelper.GetTemplate(), ThemeHelper.GetTemplateDirectory() }
        };

        foreach (var theme in themes)
        {
            var templateBasePaths = GetBlockThemeFolders(theme.Key);
            var filePath = Path.Combine(theme.Value, templateBasePaths[templateType], $"{slug}.html");
            
            if (File.Exists(filePath))
            {
                var newTemplateItem = new Dictionary<string, object>
                {
                    { "slug", slug },
                    { "path", filePath },
                    { "theme", theme.Key },
                    { "type", templateType }
                };

                if (templateType == "wp_template_part")
                {
                    return AddBlockTemplatePartAreaInfo(newTemplateItem);
                }

                if (templateType == "wp_template")
                {
                    return AddBlockTemplateInfo(newTemplateItem);
                }

                return newTemplateItem;
            }
        }

        return null;
    }

    /// <summary>
    /// Attempts to add custom template information to the template item
    /// </summary>
    internal static Dictionary<string, object> AddBlockTemplateInfo(Dictionary<string, object> templateItem)
    {
        if (!ThemeHelper.HasThemeJson())
        {
            return templateItem;
        }

        var themeData = ThemeHelper.GetThemeDataCustomTemplates();
        if (themeData.ContainsKey(templateItem["slug"].ToString()))
        {
            templateItem["title"] = themeData[templateItem["slug"].ToString()]["title"];
            templateItem["postTypes"] = themeData[templateItem["slug"].ToString()]["postTypes"];
        }

        return templateItem;
    }

    /// <summary>
    /// Attempts to add the template part's area information
    /// </summary>
    internal static Dictionary<string, object> AddBlockTemplatePartAreaInfo(Dictionary<string, object> templateInfo)
    {
        Dictionary<string, Dictionary<string, string>> themeData = null;
        if (ThemeHelper.HasThemeJson())
        {
            themeData = ThemeHelper.GetThemeDataTemplateParts();
        }

        if (themeData != null && themeData.ContainsKey(templateInfo["slug"].ToString()))
        {
            templateInfo["title"] = themeData[templateInfo["slug"].ToString()]["title"];
            templateInfo["area"] = FilterBlockTemplatePartArea(themeData[templateInfo["slug"].ToString()]["area"]);
        }
        else
        {
            templateInfo["area"] = WP_TEMPLATE_PART_AREA_UNCATEGORIZED;
        }

        return templateInfo;
    }

    // ... (other methods would be implemented similarly)
	
	public WPBlockTemplate BuildBlockTemplateResultFromPost(WPPost post)
	{
		var postId = IsPostRevision(post) ? GetPostRevisionId(post) : post.ID;
		var parentPost = GetPost(postId);

		post.PostName = parentPost.PostName;
		post.PostType = parentPost.PostType;

		var terms = GetTerms(parentPost, "wp_theme");
		if (terms == null)
		{
			throw new InvalidOperationException("No theme is defined for this template.");
		}

		var templateTerms = new Dictionary<string, string>
		{
			{ "wp_theme", terms.FirstOrDefault()?.Name }
		};

		if (parentPost.PostType == "wp_template_part")
		{
			var typeTerms = GetTerms(parentPost, "wp_template_part_area");
			if (typeTerms != null && typeTerms.Any())
			{
				templateTerms["wp_template_part_area"] = typeTerms.FirstOrDefault()?.Name;
			}
		}

		var meta = new Dictionary<string, string>
		{
			{ "origin", GetPostMeta(parentPost.ID, "origin") },
			{ "is_wp_suggestion", GetPostMeta(parentPost.ID, "is_wp_suggestion") }
		};

		var template = BuildBlockTemplateObjectFromPostObject(post, templateTerms, meta);
		return template;
	}

	public List<WPBlockTemplate> GetBlockTemplates(Dictionary<string, object> query, string templateType = "wp_template")
	{
		var templates = ApplyFilters("pre_get_block_templates", null, query, templateType);
		if (templates != null)
		{
			return templates;
		}

		var wpQueryArgs = new Dictionary<string, object>
		{
			{ "post_status", new[] { "auto-draft", "draft", "publish" } },
			{ "post_type", templateType },
			{ "posts_per_page", -1 },
			{ "no_found_rows", true },
			{ "lazy_load_term_meta", false },
			{ "tax_query", new[]
				{
					new Dictionary<string, object>
					{
						{ "taxonomy", "wp_theme" },
						{ "field", "name" },
						{ "terms", GetCurrentTheme() }
					}
				}
			}
		};

		if (templateType == "wp_template_part" && query.ContainsKey("area"))
		{
			wpQueryArgs["tax_query"] = new[]
			{
				new Dictionary<string, object>
				{
					{ "taxonomy", "wp_template_part_area" },
					{ "field", "name" },
					{ "terms", query["area"] }
				}
			};
			wpQueryArgs["tax_query_relation"] = "AND";
		}

		var templateQuery = new WPQuery(wpQueryArgs);
		var queryResult = new List<WPBlockTemplate>();

		foreach (var post in templateQuery.Posts)
		{
			var template = BuildBlockTemplateResultFromPost(post);
			if (template != null)
			{
				queryResult.Add(template);
			}
		}

		return queryResult;
	}
}


public class WPPost
{
    public int ID { get; set; }
    public string PostName { get; set; }
    public string PostType { get; set; }
    public string PostContent { get; set; }
    public DateTime PostModified { get; set; }
    public int PostAuthor { get; set; }
}

public class WPBlockTemplate
{
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Type { get; set; }
    public string Origin { get; set; }
    public string Plugin { get; set; }
}