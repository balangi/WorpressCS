using System;
using System.Collections.Generic;
using System.Linq;

public class SpeculationRulesService
{
    private readonly SpeculationRulesModel _speculationRules;

    public SpeculationRulesService()
    {
        _speculationRules = new SpeculationRulesModel();
    }

    // لیست حالت‌های مجاز
    private static readonly HashSet<string> ValidModes = new() { "prefetch", "prerender" };

    // لیست سطوح Eagerness مجاز
    private static readonly HashSet<string> ValidEagernessLevels = new() { "immediate", "eager", "moderate", "conservative" };

    // لیست منابع مجاز
    private static readonly HashSet<string> ValidSources = new() { "list", "document" };

    // اضافه کردن یک قانون جدید
    public bool AddRule(string mode, string id, Dictionary<string, object> rule)
    {
        // اعتبارسنجی حالت
        if (!IsValidMode(mode))
        {
            Console.WriteLine($"Invalid mode: {mode}");
            return false;
        }

        // اعتبارسنجی شناسه
        if (!IsValidId(id))
        {
            Console.WriteLine($"Invalid ID: {id}");
            return false;
        }

        // بررسی وجود قانون با شناسه مشابه
        if (HasRule(mode, id))
        {
            Console.WriteLine($"A rule with ID '{id}' already exists.");
            return false;
        }

        // اعتبارسنجی ساختار قانون
        if (!ValidateRuleStructure(rule))
        {
            Console.WriteLine("Invalid rule structure.");
            return false;
        }

        // اضافه کردن قانون
        _speculationRules.AddRule(mode, id, rule);
        return true;
    }

    // بررسی وجود قانون
    public bool HasRule(string mode, string id)
    {
        return _speculationRules.RulesByMode.ContainsKey(mode) &&
               _speculationRules.RulesByMode[mode].ContainsKey(id);
    }

    // اعتبارسنجی حالت
    private bool IsValidMode(string mode)
    {
        return ValidModes.Contains(mode);
    }

    // اعتبارسنجی شناسه
    private bool IsValidId(string id)
    {
        return !string.IsNullOrEmpty(id) && System.Text.RegularExpressions.Regex.IsMatch(id, @"^[a-z][a-z0-9_-]+$");
    }

    // اعتبارسنجی ساختار قانون
    private bool ValidateRuleStructure(Dictionary<string, object> rule)
    {
        // باید یکی از 'where' یا 'urls' وجود داشته باشد، اما نه هر دو
        bool hasWhere = rule.ContainsKey("where");
        bool hasUrls = rule.ContainsKey("urls");

        if ((hasWhere && hasUrls) || (!hasWhere && !hasUrls))
        {
            return false;
        }

        // اعتبارسنجی منبع
        if (rule.ContainsKey("source") && !ValidSources.Contains(rule["source"].ToString()))
        {
            return false;
        }

        // اعتبارسنجی Eagerness
        if (rule.ContainsKey("eagerness") && !ValidEagernessLevels.Contains(rule["eagerness"].ToString()))
        {
            return false;
        }

        // بررسی محدودیت‌های خاص
        if (rule.ContainsKey("source"))
        {
            string source = rule["source"].ToString();
            if (source == "list" && hasWhere)
            {
                return false;
            }

            if (source == "document" && hasUrls)
            {
                return false;
            }
        }

        if (rule.ContainsKey("eagerness") && rule["eagerness"].ToString() == "immediate" && hasWhere)
        {
            return false;
        }

        return true;
    }

    // دریافت داده‌ها به صورت JSON
    public Dictionary<string, List<Dictionary<string, object>>> GetJsonData()
    {
        var result = new Dictionary<string, List<Dictionary<string, object>>>();

        foreach (var mode in _speculationRules.RulesByMode.Keys)
        {
            result[mode] = _speculationRules.RulesByMode[mode]
                .Values
                .Select(rule => rule.RuleData)
                .ToList();
        }

        return result;
    }
}