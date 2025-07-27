using System;
using System.Collections.Generic;

public class RewriteRule
{
    public int Id { get; set; }
    public string RegexPattern { get; set; }
    public string Query { get; set; }
    public string Priority { get; set; } // e.g., "top", "bottom"
}

public class Endpoint
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Places { get; set; } // Bitmask for places (e.g., EP_PERMALINK, EP_DATE)
    public string QueryVar { get; set; }
}

public class Permastruct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Structure { get; set; }
    public Dictionary<string, object> Args { get; set; } = new Dictionary<string, object>();
}