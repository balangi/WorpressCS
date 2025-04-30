using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class HttpManager
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly ILogger<HttpManager> _logger;

    public HttpManager(HttpClient httpClient, AppDbContext context, ILogger<HttpManager> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
    }

    // ارسال درخواست HTTP
    public async Task<HttpResponseMessage> SendRequestAsync(string url, HttpMethod method, Dictionary<string, string> headers = null, object data = null)
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