using System;
using System.Collections.Generic;

public class Shortcode
{
    public string Tag { get; set; }
    public Func<Dictionary<string, string>, string, string> Callback { get; set; }
}

public class ShortcodeContent
{
    public string Content { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
}