using System;
using System.Linq;
using Microsoft.Extensions.Logging;

public class HttpCookie
{
    private readonly ILogger<HttpCookie> _logger;

    public string Name { get; private set; }
    public string Value { get; private set; }
    public DateTime? Expires { get; private set; }
    public string Path { get; private set; }
    public string Domain { get; private set; }
    public string Port { get; private set; }
    public bool HostOnly { get; private set; }

    public HttpCookie(object data, string requestedUrl = "", ILogger<HttpCookie> logger = null)
    {
        _logger = logger;

        if (!string.IsNullOrEmpty(requestedUrl))
        {
            var parsedUrl = new Uri(requestedUrl);
            Domain = parsedUrl.Host;
            Path = parsedUrl.AbsolutePath.EndsWith("/") ? parsedUrl.AbsolutePath : $"{parsedUrl.AbsolutePath}/";
        }

        if (data is string header)
        {
            ParseHeader(header);
        }
        else if (data is System.Collections.IDictionary dict)
        {
            ParseDictionary(dict);
        }
        else
        {
            throw new ArgumentException("Invalid data format for cookie.");
        }
    }

    private void ParseHeader(string header)
    {
        var pairs = header.Split(';');
        var firstPair = pairs[0].Split('=');
        Name = firstPair[0].Trim();
        Value = Uri.UnescapeDataString(firstPair[1]);

        foreach (var pair in pairs.Skip(1))
        {
            var parts = pair.Split('=');
            var key = parts[0].Trim().ToLower();
            var value = parts.Length > 1 ? parts[1].Trim() : null;

            switch (key)
            {
                case "expires":
                    Expires = DateTime.TryParse(value, out var expires) ? expires : (DateTime?)null;
                    break;
                case "path":
                    Path = value;
                    break;
                case "domain":
                    Domain = value;
                    break;
                case "port":
                    Port = value;
                    break;
                case "host_only":
                    HostOnly = bool.Parse(value);
                    break;
            }
        }
    }

    private void ParseDictionary(System.Collections.IDictionary data)
    {
        Name = data["name"]?.ToString();
        Value = data["value"]?.ToString();
        Expires = data["expires"] != null ? (data["expires"] is int timestamp ? DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime : DateTime.Parse(data["expires"].ToString())) : (DateTime?)null;
        Path = data["path"]?.ToString() ?? "/";
        Domain = data["domain"]?.ToString();
        Port = data["port"]?.ToString();
        HostOnly = data["host_only"] != null && bool.Parse(data["host_only"].ToString());
    }

    public bool Test(string url)
    {
        if (string.IsNullOrEmpty(Name))
        {
            return false;
        }

        if (Expires.HasValue && DateTime.UtcNow > Expires.Value)
        {
            return false;
        }

        var parsedUrl = new Uri(url);
        var urlPort = parsedUrl.Port > 0 ? parsedUrl.Port : (parsedUrl.Scheme == "https" ? 443 : 80);
        var urlPath = parsedUrl.AbsolutePath;

        var domain = Domain ?? parsedUrl.Host;
        if (!domain.Contains("."))
        {
            domain += ".local";
        }

        domain = domain.StartsWith(".") ? domain.Substring(1) : domain;

        if (!parsedUrl.Host.EndsWith(domain))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(Port) && !Port.Split(',').Select(int.Parse).Contains(urlPort))
        {
            return false;
        }

        if (!urlPath.StartsWith(Path))
        {
            return false;
        }

        return true;
    }

    public string GetHeaderValue()
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Value))
        {
            return string.Empty;
        }

        return $"{Name}={Value}";
    }

    public string GetFullHeader()
    {
        return $"Cookie: {GetHeaderValue()}";
    }

    public object GetAttributes()
    {
        return new
        {
            Expires,
            Path,
            Domain
        };
    }
}