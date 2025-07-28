using System;
using System.Collections.Generic;

public class SpeculationRulesConfiguration
{
    public string Mode { get; set; } // "prefetch" or "prerender"
    public string Eagerness { get; set; } // "eager", "moderate", or "conservative"
}

public class SpeculationRule
{
    public string Mode { get; set; }
    public string Source { get; set; }
    public Dictionary<string, object> Where { get; set; } = new Dictionary<string, object>();
    public string Eagerness { get; set; }
}

public class SpeculationRules
{
    public List<SpeculationRule> Rules { get; set; } = new List<SpeculationRule>();

    public void AddRule(string mode, string source, Dictionary<string, object> conditions, string eagerness)
    {
        Rules.Add(new SpeculationRule
        {
            Mode = mode,
            Source = source,
            Where = conditions,
            Eagerness = eagerness
        });
    }
}