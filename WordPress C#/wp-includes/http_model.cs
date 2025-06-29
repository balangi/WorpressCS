using System;
using System.Collections.Generic;

public class HttpRequest
{
    public string Url { get; set; }
    public HttpMethod Method { get; set; } = HttpMethod.GET;
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>();
    public string Body { get; set; }
}

public class HttpResponse
{
    public int StatusCode { get; set; } = 200;
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public string Body { get; set; }
}

public enum HttpMethod
{
    GET,
    POST,
    PUT,
    DELETE,
    OPTIONS
}