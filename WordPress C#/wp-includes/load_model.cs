using System;
using System.Collections.Generic;

public class ServerSettings
{
    public string ServerSoftware { get; set; }
    public string RequestUri { get; set; }
    public string ScriptFilename { get; set; }
    public string PathInfo { get; set; }
    public string QueryString { get; set; }
}

public class PhpVersionRequirement
{
    public string RequiredPhpVersion { get; set; }
    public List<string> RequiredExtensions { get; set; } = new List<string>();
}