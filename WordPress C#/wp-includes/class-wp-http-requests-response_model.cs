public class HttpResponse
{
    public int StatusCode { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public List<HttpResponseCookie> Cookies { get; set; } = new();
}

public class HttpResponseCookie
{
    public string Name { get; set; }
    public string Value { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
    public Dictionary<string, bool> Flags { get; set; } = new();
}

public class HttpCookie
{
    public string Name { get; set; }
    public string Value { get; set; }
    public DateTime? Expires { get; set; }
    public string Path { get; set; }
    public string Domain { get; set; }
    public bool HostOnly { get; set; }
}