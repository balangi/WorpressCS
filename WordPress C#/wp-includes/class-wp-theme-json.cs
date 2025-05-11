using System;
using System.Collections.Generic;
using System.Linq;

public class WP_Theme_JSON
{
    // داده‌های JSON مربوط به Theme
    public Dictionary<string, object> ThemeJson { get; private set; }

    // سازنده کلاس
    public WP_Theme_JSON(Dictionary<string, object> themeJson)
    {
        ThemeJson = themeJson ?? new Dictionary<string, object>();
    }

    // دریافت داده‌های خام JSON
    public Dictionary<string, object> GetRawData()
    {
        return ThemeJson;
    }

    // تنظیم مقادیر در مسیر مشخص
    public void SetPropertyValue(string[] path, object value)
    {
        var current = ThemeJson;

        for (int i = 0; i < path.Length - 1; i++)
        {
            if (!current.ContainsKey(path[i]))
            {
                current[path[i]] = new Dictionary<string, object>();
            }

            current = current[path[i]] as Dictionary<string, object>;
        }

        current[path[^1]] = value;
    }

    // دریافت مقادیر از مسیر مشخص
    public object GetPropertyValue(string[] path)
    {
        var current = ThemeJson;

        foreach (var key in path)
        {
            if (current == null || !current.ContainsKey(key))
            {
                return null;
            }

            current = current[key] as Dictionary<string, object>;
        }

        return current;
    }

    // محاسبه اندازه‌های فاصله‌گذاری
    public List<Dictionary<string, object>> ComputeSpacingSizes(Dictionary<string, object> spacingScale)
    {
        var spacingSizes = new List<Dictionary<string, object>>();

        if (spacingScale == null || !spacingScale.ContainsKey("steps"))
        {
            return spacingSizes;
        }

        int steps = Convert.ToInt32(spacingScale["steps"]);
        double increment = Convert.ToDouble(spacingScale["increment"]);
        double unit = Convert.ToDouble(spacingScale["unit"]);

        for (int i = 1; i <= steps; i++)
        {
            spacingSizes.Add(new Dictionary<string, object>
            {
                { "slug", $"spacing-{i}" },
                { "size", $"{i * increment}{unit}" }
            });
        }

        return spacingSizes;
    }

    // ادغام دو لیست از اندازه‌های فاصله‌گذاری
    public List<Dictionary<string, object>> MergeSpacingSizes(
        List<Dictionary<string, object>> baseSizes,
        List<Dictionary<string, object>> incomingSizes)
    {
        var mergedSizes = new Dictionary<string, Dictionary<string, object>>();

        foreach (var size in baseSizes)
        {
            mergedSizes[size["slug"].ToString()] = size;
        }

        foreach (var size in incomingSizes)
        {
            mergedSizes[size["slug"].ToString()] = size;
        }

        return mergedSizes.Values.ToList();
    }

    // حذف مقادیر غیرمجاز
    public Dictionary<string, object> RemoveInsecureProperties(Dictionary<string, object> themeJson)
    {
        var sanitized = new Dictionary<string, object>();

        foreach (var entry in themeJson)
        {
            if (entry.Value is Dictionary<string, object> nestedDict)
            {
                sanitized[entry.Key] = RemoveInsecureProperties(nestedDict);
            }
            else
            {
                sanitized[entry.Key] = entry.Value;
            }
        }

        return sanitized;
    }

    // تبدیل متغیرها به مقادیر
    public Dictionary<string, object> ConvertVariablesToValue(Dictionary<string, object> styles, Dictionary<string, string> vars)
    {
        var convertedStyles = new Dictionary<string, object>();

        foreach (var entry in styles)
        {
            if (entry.Value is string value && vars.ContainsKey(value))
            {
                convertedStyles[entry.Key] = vars[value];
            }
            else if (entry.Value is Dictionary<string, object> nestedDict)
            {
                convertedStyles[entry.Key] = ConvertVariablesToValue(nestedDict, vars);
            }
            else
            {
                convertedStyles[entry.Key] = entry.Value;
            }
        }

        return convertedStyles;
    }
}