using System;
using System.Collections.Generic;

public class SpeculationRule
{
    public string Id { get; set; } // شناسه منحصر به فرد قانون
    public string Mode { get; set; } // حالت قانون (prefetch یا prerender)
    public Dictionary<string, object> RuleData { get; set; } // داده‌های قانون
}

public class SpeculationRulesModel
{
    public Dictionary<string, Dictionary<string, SpeculationRule>> RulesByMode { get; set; } = new();

    public void AddRule(string mode, string id, Dictionary<string, object> rule)
    {
        if (!RulesByMode.ContainsKey(mode))
        {
            RulesByMode[mode] = new Dictionary<string, SpeculationRule>();
        }

        RulesByMode[mode][id] = new SpeculationRule
        {
            Id = id,
            Mode = mode,
            RuleData = rule
        };
    }
}