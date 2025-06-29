using System;
using System.Collections.Generic;

public class GlobalSetting
{
    public string Path { get; set; }
    public object Value { get; set; }
}

public class GlobalStyle
{
    public string BlockName { get; set; }
    public Dictionary<string, object> Styles { get; set; } = new Dictionary<string, object>();
}

public class ThemeJsonData
{
    public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
    public Dictionary<string, object> Styles { get; set; } = new Dictionary<string, object>();
}

public class CachedData
{
    public string CacheKey { get; set; }
    public object Data { get; set; }
}