using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class HttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Sends an HTTP request and retrieves the response.
    /// </summary>
    public async Task<HttpResponse> SendRequestAsync(HttpRequest request)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = GetHttpMethod(request.Method),
            RequestUri = new Uri(request.Url)
        };

        // Add headers
        foreach (var header in request.Headers)
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Add query parameters
        var uriBuilder = new UriBuilder(request.Url);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        foreach (var param in request.QueryParameters)
        {
            query[param.Key] = param.Value;
        }
        uriBuilder.Query = query.ToString();
        httpRequestMessage.RequestUri = uriBuilder.Uri;

        // Add body for POST/PUT requests
        if (request.Method == HttpMethod.POST || request.Method == HttpMethod.PUT)
        {
            httpRequestMessage.Content = new StringContent(request.Body ?? string.Empty);
        }

        // Send the request
        var response = await _httpClient.SendAsync(httpRequestMessage);

        // Parse the response
        var httpResponse = new HttpResponse
        {
            StatusCode = (int)response.StatusCode,
            Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
            Body = await response.Content.ReadAsStringAsync()
        };

        return httpResponse;
    }

    /// <summary>
    /// Validates a URL for safe use in HTTP requests.
    /// </summary>
    public bool ValidateUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return false;

        try
        {
            var uri = new Uri(url);
            if (!uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
                !uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                return false;

            if (IsLocalOrRestrictedIp(uri.Host))
                return false;

            if (!IsAllowedPort(uri.Port))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the IP address is local or restricted.
    /// </summary>
    private bool IsLocalOrRestrictedIp(string host)
    {
        var ipAddresses = System.Net.Dns.GetHostAddresses(host);
        foreach (var ip in ipAddresses)
        {
            var address = ip.ToString();
            if (address.StartsWith("127.") || address.StartsWith("10.") ||
                address.StartsWith("192.168.") || address.StartsWith("172."))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the port is allowed.
    /// </summary>
    private bool IsAllowedPort(int port)
    {
        var allowedPorts = new List<int> { 80, 443, 8080 };
        return allowedPorts.Contains(port);
    }

    /// <summary>
    /// Maps HttpMethod enum to HttpClient method.
    /// </summary>
    private HttpMethod GetHttpMethod(HttpMethod method)
    {
        return method switch
        {
            HttpMethod.GET => HttpMethod.Get,
            HttpMethod.POST => HttpMethod.Post,
            HttpMethod.PUT => HttpMethod.Put,
            HttpMethod.DELETE => HttpMethod.Delete,
            HttpMethod.OPTIONS => HttpMethod.Options,
            _ => throw new ArgumentException("Unsupported HTTP method.")
        };
    }
}