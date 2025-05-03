using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class HttpIxrClient
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly ILogger<HttpIxrClient> _logger;

    public string Scheme { get; private set; }
    public string Server { get; private set; }
    public string Path { get; private set; }
    public int? Port { get; private set; }
    public string UserAgent { get; private set; }
    public int Timeout { get; private set; }
    public Dictionary<string, string> Headers { get; private set; } = new();
    public IXRError Error { get; private set; }

    public HttpIxrClient(string server, string path = null, int? port = null, int timeout = 15, ILogger<HttpIxrClient> logger = null)
    {
        _logger = logger;

        if (!string.IsNullOrEmpty(path))
        {
            Scheme = "http";
            Server = server;
            Path = path;
            Port = port;
        }
        else
        {
            var parsedUrl = new Uri(server);
            Scheme = parsedUrl.Scheme;
            Server = parsedUrl.Host;
            Port = parsedUrl.Port > 0 ? parsedUrl.Port : port;
            Path = !string.IsNullOrEmpty(parsedUrl.AbsolutePath) ? parsedUrl.AbsolutePath : "/";
            if (!string.IsNullOrEmpty(parsedUrl.Query))
            {
                Path += parsedUrl.Query;
            }
        }

        UserAgent = "The Incutio XML-RPC C# Library";
        Timeout = timeout;
    }

    public async Task<bool> QueryAsync(string method, params object[] args)
    {
        try
        {
            var request = new IXRRequest(method, args);
            var xml = request.GetXml();

            var url = $"{Scheme}://{Server}:{Port}{Path}";
            var httpContent = new StringContent(xml, System.Text.Encoding.UTF8, "text/xml");

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, url))
            {
                httpRequest.Content = httpContent;
                httpRequest.Headers.Add("User-Agent", UserAgent);

                foreach (var header in Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }

                var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    Error = new IXRError(-32301, $"Transport error - HTTP status code was not 200 ({(int)response.StatusCode})");
                    return false;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var message = new IXRMessage(responseBody);

                if (!message.Parse())
                {
                    Error = new IXRError(-32700, "Parse error. Not well formed.");
                    return false;
                }

                if (message.MessageType == "fault")
                {
                    Error = new IXRError(message.FaultCode, message.FaultString);
                    return false;
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Failed to send XML-RPC request: {ex.Message}");
            Error = new IXRError(-32300, $"Transport error: {ex.Message}");
            return false;
        }
    }
}

// کلاس‌های کمکی
public class IXRRequest
{
    public string Method { get; }
    public object[] Args { get; }

    public IXRRequest(string method, object[] args)
    {
        Method = method;
        Args = args;
    }

    public string GetXml()
    {
        // تولید XML برای درخواست XML-RPC
        return $"<methodCall><methodName>{Method}</methodName><params>{string.Join("", Array.ConvertAll(Args, arg => $"<param><value>{arg}</value></param>"))}</params></methodCall>";
    }
}

public class IXRMessage
{
    public string MessageType { get; private set; }
    public int FaultCode { get; private set; }
    public string FaultString { get; private set; }

    private readonly string _xml;

    public IXRMessage(string xml)
    {
        _xml = xml;
    }

    public bool Parse()
    {
        try
        {
            var document = System.Xml.Linq.XDocument.Parse(_xml);
            MessageType = document.Root.Element("fault") != null ? "fault" : "response";

            if (MessageType == "fault")
            {
                var fault = document.Root.Element("fault").Element("value").Element("struct");
                FaultCode = int.Parse(fault.Element("member").Element("value").Value);
                FaultString = fault.Element("member").Element("value").Value;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class IXRError
{
    public int Code { get; }
    public string Message { get; }

    public IXRError(int code, string message)
    {
        Code = code;
        Message = message;
    }
}