using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class HttpStreamManager
{
    private readonly ILogger<HttpStreamManager> _logger;

    public HttpStreamManager(ILogger<HttpStreamManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// ارسال درخواست HTTP با استفاده از HttpClient
    /// </summary>
    public async Task<HttpResponseMessage> SendRequestAsync(string url, HttpRequestOptions options)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                // تنظیمات پروکسی
                if (options.Proxy != null && options.Proxy.IsEnabled())
                {
                    var proxyUri = new Uri($"http://{options.Proxy.Host}:{options.Proxy.Port}");
                    var handler = new HttpClientHandler
                    {
                        Proxy = new System.Net.WebProxy(proxyUri, true),
                        UseProxy = true
                    };
                    httpClient = new HttpClient(handler);
                }

                // تنظیمات SSL
                if (options.SslVerify)
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                }

                // ارسال درخwaست
                var response = await httpClient.GetAsync(url);

                // بررسی وضعیت پاسخ
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"HTTP request failed with status code: {response.StatusCode}");
                    return response;
                }

                // ذخیره‌سازی پاسخ در فایل (اگر نیاز باشد)
                if (options.StreamToFile)
                {
                    await SaveResponseToFileAsync(response, options.Filename);
                }

                return response;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending HTTP request: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// ذخیره‌سازی پاسخ در فایل
    /// </summary>
    private async Task SaveResponseToFileAsync(HttpResponseMessage response, string filename)
    {
        try
        {
            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                await response.Content.CopyToAsync(fileStream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to write response to file: {ex.Message}");
            throw;
        }
    }
}
