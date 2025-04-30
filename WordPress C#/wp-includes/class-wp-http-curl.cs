using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class HttpCurlManager
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly ILogger<HttpCurlManager> _logger;

    public HttpCurlManager(HttpClient httpClient, AppDbContext context, ILogger<HttpCurlManager> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
    }

    // ارسال درخواست HTTP
    public async Task<HttpResponseMessage> SendRequestAsync(string url, HttpMethod method, Dictionary<string, string> headers = null, object data = null, bool stream = false, string filename = null)
    {
        try
        {
            var request = new HttpRequestMessage(method, url);

            // افزودن هدرها
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            // افزودن داده‌ها به بدنه درخواست
            if (data != null)
            {
                var jsonData = JsonSerializer.Serialize(data);
                request.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            }

            // ارسال درخواست
            var response = await _httpClient.SendAsync(request);

            // مدیریت جریان‌سازی (streaming)
            if (stream && !string.IsNullOrEmpty(filename))
            {
                using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }

            // ثبت اطلاعات درخواست در دیتابیس
            LogRequest(url, method.Method, (int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send HTTP request: {ex.Message}");
            throw;
        }
    }

    // پردازش هدرها
    public Dictionary<string, string> ProcessHeaders(HttpResponseMessage response)
    {
        var headers = new Dictionary<string, string>();

        foreach (var header in response.Headers)
        {
            headers[header.Key] = string.Join(", ", header.Value);
        }

        return headers;
    }

    // مدیریت SSL
    public void ConfigureSsl(bool sslVerify)
    {
        _httpClient.DefaultRequestVersion = new Version(2, 0); // HTTP/2
        _httpClient.DefaultRequestHeaders.ExpectContinue = false;

        if (!sslVerify)
        {
            _httpClient.DefaultRequestHeaders.Add("DisableSslVerification", "true");
        }
    }

    // مدیریت پروکسی
    public void ConfigureProxy(string proxyUrl, string proxyAuth = null)
    {
        var handler = new HttpClientHandler
        {
            Proxy = new System.Net.WebProxy(proxyUrl, true),
            UseProxy = true
        };

        if (!string.IsNullOrEmpty(proxyAuth))
        {
            handler.Proxy.Credentials = new System.Net.NetworkCredential("username", "password");
        }

        _httpClient = new HttpClient(handler);
    }

    // ثبت اطلاعات درخواست در دیتابیس
    private void LogRequest(string url, string method, int statusCode, string response)
    {
        var log = new HttpRequestLog
        {
            Url = url,
            Method = method,
            RequestedAt = DateTime.UtcNow,
            StatusCode = statusCode,
            Response = response
        };

        _context.HttpRequestLogs.Add(log);
        _context.SaveChanges();
    }
}