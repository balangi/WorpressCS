using System;
using System.Collections.Generic;
using System.Linq;

public class WP_Theme_JSON_Schema
{
    // مسیرهای تغییر نام‌یافته از v1 به v2
    private static readonly Dictionary<string, string> V1ToV2RenamedPaths = new()
    {
        { "border.customRadius", "border.radius" },
        { "spacing.customMargin", "spacing.margin" },
        { "spacing.customPadding", "spacing.padding" },
        { "typography.customLineHeight", "typography.lineHeight" }
    };

    /// <summary>
    /// مهاجرت داده‌ها به آخرین نسخه اسکیما.
    /// </summary>
    /// <param name="themeJson">داده‌های theme.json.</param>
    /// <param name="origin">منبع داده‌ها (مثل 'blocks', 'default', 'theme', 'custom').</param>
    /// <returns>داده‌های مهاجرت‌شده به آخرین نسخه.</returns>
    public static Dictionary<string, object> Migrate(Dictionary<string, object> themeJson, string origin = "theme")
    {
        if (!themeJson.ContainsKey("version"))
        {
            themeJson["version"] = WP_Theme_JSON.LatestSchema;
        }

        // مهاجرت هر نسخه به ترتیب
        switch ((int)themeJson["version"])
        {
            case 1:
                themeJson = MigrateV1ToV2(themeJson);
                goto case 2; // پس از مهاجرت به v2، به v3 نیز مهاجرت کن.
            case 2:
                themeJson = MigrateV2ToV3(themeJson, origin);
                break;
        }

        return themeJson;
    }

    /// <summary>
    /// مهاجرت از v1 به v2.
    /// </summary>
    /// <param name="oldData">داده‌های قدیمی.</param>
    /// <returns>داده‌های مهاجرت‌شده به v2.</returns>
    private static Dictionary<string, object> MigrateV1ToV2(Dictionary<string, object> oldData)
    {
        var newData = new Dictionary<string, object>(oldData);

        if (newData.ContainsKey("settings"))
        {
            newData["settings"] = RenamePaths(newData["settings"] as Dictionary<string, object>, V1ToV2RenamedPaths);
        }

        newData["version"] = 2;
        return newData;
    }

    /// <summary>
    /// مهاجرت از v2 به v3.
    /// </summary>
    /// <param name="oldData">داده‌های قدیمی.</param>
    /// <param name="origin">منبع داده‌ها.</param>
    /// <returns>داده‌های مهاجرت‌شده به v3.</returns>
    private static Dictionary<string, object> MigrateV2ToV3(Dictionary<string, object> oldData, string origin)
    {
        var newData = new Dictionary<string, object>(oldData);

        // تنظیم نسخه جدید
        newData["version"] = 3;

        // تغییرات برای منابع غیر custom
        if (origin != "custom")
        {
            if (HasPath(newData, "settings.typography.fontSizes"))
            {
                SetPathValue(newData, "settings.typography.defaultFontSizes", false);
            }

            if (HasPath(newData, "settings.spacing.spacingSizes") || HasPath(newData, "settings.spacing.spacingScale"))
            {
                SetPathValue(newData, "settings.spacing.defaultSpacingSizes", false);
            }

            if (HasPath(newData, "settings.spacing.spacingSizes"))
            {
                RemovePath(newData, "settings.spacing.spacingScale");
            }
        }

        return newData;
    }

    /// <summary>
    /// پردازش مسیرهای تنظیمات.
    /// </summary>
    /// <param name="settings">تنظیمات.</param>
    /// <param name="pathsToRename">مسیرهایی که باید تغییر نام دهند.</param>
    /// <returns>تنظیمات به‌روزرسانی‌شده.</returns>
    private static Dictionary<string, object> RenamePaths(Dictionary<string, object> settings, Dictionary<string, string> pathsToRename)
    {
        var newSettings = new Dictionary<string, object>(settings);

        // تغییر نام تنظیمات پیش‌فرض
        RenameSettings(newSettings, pathsToRename);

        // تغییر نام تنظیمات بلوک‌ها
        if (newSettings.ContainsKey("blocks") && newSettings["blocks"] is Dictionary<string, object> blocks)
        {
            foreach (var block in blocks)
            {
                if (block.Value is Dictionary<string, object> blockSettings)
                {
                    RenameSettings(blockSettings, pathsToRename);
                }
            }
        }

        return newSettings;
    }

    /// <summary>
    /// تغییر نام تنظیمات.
    /// </summary>
    /// <param name="settings">تنظیمات.</param>
    /// <param name="pathsToRename">مسیرهایی که باید تغییر نام دهند.</param>
    private static void RenameSettings(Dictionary<string, object> settings, Dictionary<string, string> pathsToRename)
    {
        foreach (var pathPair in pathsToRename)
        {
            var originalPath = pathPair.Key.Split('.');
            var renamedPath = pathPair.Value.Split('.');
            var currentValue = GetPathValue(settings, originalPath);

            if (currentValue != null)
            {
                SetPathValue(settings, renamedPath, currentValue);
                RemovePath(settings, originalPath);
            }
        }
    }

    /// <summary>
    /// حذف تنظیمات بر اساس مسیر.
    /// </summary>
    /// <param name="settings">تنظیمات.</param>
    /// <param name="path">مسیر.</param>
    private static void RemovePath(Dictionary<string, object> settings, string[] path)
    {
        var tmpSettings = settings;
        var lastKey = path[^1];
        foreach (var key in path.Take(path.Length - 1))
        {
            if (tmpSettings.ContainsKey(key) && tmpSettings[key] is Dictionary<string, object> nestedDict)
            {
                tmpSettings = nestedDict;
            }
            else
            {
                return; // مسیر وجود ندارد
            }
        }

        tmpSettings.Remove(lastKey);
    }

    /// <summary>
    /// دریافت مقدار بر اساس مسیر.
    /// </summary>
    /// <param name="data">داده‌ها.</param>
    /// <param name="path">مسیر.</param>
    /// <returns>مقدار مورد نظر.</returns>
    private static object GetPathValue(Dictionary<string, object> data, string[] path)
    {
        var current = data;
        foreach (var key in path)
        {
            if (current.ContainsKey(key) && current[key] is Dictionary<string, object> nestedDict)
            {
                current = nestedDict;
            }
            else
            {
                return null; // مسیر وجود ندارد
            }
        }

        return current;
    }

    /// <summary>
    /// تنظیم مقدار بر اساس مسیر.
    /// </summary>
    /// <param name="data">داده‌ها.</param>
    /// <param name="path">مسیر.</param>
    /// <param name="value">مقدار جدید.</param>
    private static void SetPathValue(Dictionary<string, object> data, string[] path, object value)
    {
        var current = data;
        foreach (var key in path.Take(path.Length - 1))
        {
            if (!current.ContainsKey(key) || !(current[key] is Dictionary<string, object> nestedDict))
            {
                nestedDict = new();
                current[key] = nestedDict;
            }

            current = nestedDict;
        }

        current[path[^1]] = value;
    }

    /// <summary>
    /// بررسی وجود مسیر.
    /// </summary>
    /// <param name="data">داده‌ها.</param>
    /// <param name="path">مسیر.</param>
    /// <returns>آیا مسیر وجود دارد؟</returns>
    private static bool HasPath(Dictionary<string, object> data, string path)
    {
        return GetPathValue(data, path.Split('.')) != null;
    }
}