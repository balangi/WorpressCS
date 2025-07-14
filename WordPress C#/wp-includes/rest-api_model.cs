using System;
using System.Collections.Generic;

public class RestRoute
{
    public string Path { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
}

public class RestResponse
{
    public int StatusCode { get; set; }
    public object Data { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}