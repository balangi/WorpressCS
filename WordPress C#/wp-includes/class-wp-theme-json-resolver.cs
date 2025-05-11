using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class WP_Theme_JSON_Resolver
{
    // ذخیره داده‌های Core
    private static WP_Theme_JSON Core { get; set; }

    // ذخیره داده‌های Theme
    private static WP_Theme_JSON Theme { get; set; }

    // ذخیره داده‌های User
    private static WP_Theme_JSON User { get; set; }

    // ذخیره داده‌های Block
    private static WP_Theme_JSON Blocks { get; set; }

    // ذخیره شماژ i18n
    private static Dictionary<string, object> I18nSchema { get; set; }

    // ذخیره Cache فایل‌های theme.json
    private static Dictionary<string, Dictionary<string, object>> ThemeJsonFileCache { get; set; } = new();

    // خواندن فایل JSON
    public static Dictionary<string, object> ReadJsonFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new Dictionary<string, object>();
        }

        var json = File.ReadAllText(filePath);
        return JsonHelper.Deserialize<Dictionary<string, object>>(json);
    }

    // ترجمه داده‌های JSON
    public static Dictionary<string, object> Translate(Dictionary<string, object> themeJson, string domain = "default")
    {
        if (I18nSchema == null)
        {
            var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme-i18n.json");
            I18nSchema = ReadJsonFile(schemaPath);
        }

        return TranslateSettingsUsingI18nSchema(I18nSchema, themeJson, domain);
    }

    // ترجمه تنظیمات با استفاده از شماژ i18n
    private static Dictionary<string, object> TranslateSettingsUsingI18nSchema(
        Dictionary<string, object> i18nSchema,
        Dictionary<string, object> themeJson,
        string domain)
    {
        var translated = new Dictionary<string, object>();

        foreach (var key in themeJson.Keys)
        {
            if (i18nSchema.ContainsKey(key) && themeJson[key] is string value)
            {
                translated[key] = TranslateString(value, domain);
            }
            else if (themeJson[key] is Dictionary<string, object> nestedDict)
            {
                translated[key] = TranslateSettingsUsingI18nSchema(i18nSchema, nestedDict, domain);
            }
            else
            {
                translated[key] = themeJson[key];
            }
        }

        return translated;
    }

    // ترجمه رشته‌ها
    private static string TranslateString(string value, string domain)
    {
        // منطق ترجمه را اینجا پیاده‌سازی کنید
        return value; // به عنوان نمونه، مقدار اصلی برگردانده می‌شود
    }

    // دریافت داده‌های Core
    public static WP_Theme_JSON GetCoreData()
    {
        if (Core != null)
        {
            return Core;
        }

        var coreFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme.json");
        var coreData = ReadJsonFile(coreFilePath);
        coreData = Translate(coreData);

        Core = new WP_Theme_JSON(coreData, "core");
        return Core;
    }

    // دریافت داده‌های Theme
    public static WP_Theme_JSON GetThemeData(WP_Theme theme)
    {
        if (Theme != null)
        {
            return Theme;
        }

        var themeFilePath = Path.Combine(theme.ThemeRoot, theme.Stylesheet, "theme.json");
        var themeData = ReadJsonFile(themeFilePath);

        if (theme.Parent() != null)
        {
            var parentThemeFilePath = Path.Combine(theme.Parent().ThemeRoot, theme.Parent().Stylesheet, "theme.json");
            var parentThemeData = ReadJsonFile(parentThemeFilePath);

            var parentTheme = new WP_Theme_JSON(parentThemeData, "parent");
            var currentTheme = new WP_Theme_JSON(themeData, "theme");

            parentTheme.Merge(currentTheme);
            Theme = parentTheme;
        }
        else
        {
            Theme = new WP_Theme_JSON(themeData, "theme");
        }

        return Theme;
    }

    // دریافت داده‌های User
    public static WP_Theme_JSON GetUserData(WP_Theme theme)
    {
        if (User != null)
        {
            return User;
        }

        var userCpt = GetUserPostDataFromGlobalStyles(theme);
        if (userCpt.ContainsKey("post_content"))
        {
            var decodedData = JsonHelper.Deserialize<Dictionary<string, object>>(userCpt["post_content"].ToString());
            if (decodedData.ContainsKey("isGlobalStylesUserThemeJSON"))
            {
                decodedData.Remove("isGlobalStylesUserThemeJSON");
            }

            User = new WP_Theme_JSON(decodedData, "user");
        }

        return User;
    }

    // دریافت داده‌های Block
    public static WP_Theme_JSON GetBlocksData()
    {
        if (Blocks != null)
        {
            return Blocks;
        }

        var blocksFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "blocks.json");
        var blocksData = ReadJsonFile(blocksFilePath);

        Blocks = new WP_Theme_JSON(blocksData, "blocks");
        return Blocks;
    }

    // دریافت داده‌های ادغام‌شده
    public static WP_Theme_JSON GetMergedData()
    {
        var coreData = GetCoreData();
        var themeData = GetThemeData(new WP_Theme("active-theme", "/path/to/theme"));
        var userData = GetUserData(new WP_Theme("active-theme", "/path/to/theme"));
        var blocksData = GetBlocksData();

        var mergedData = new WP_Theme_JSON(new Dictionary<string, object>(), "merged");
        mergedData.Merge(coreData);
        mergedData.Merge(themeData);
        mergedData.Merge(userData);
        mergedData.Merge(blocksData);

        return mergedData;
    }

    // دریافت داده‌های Post از Global Styles
    private static Dictionary<string, object> GetUserPostDataFromGlobalStyles(WP_Theme theme)
    {
        // منطق دریافت داده‌ها از Global Styles
        return new Dictionary<string, object>();
    }
}