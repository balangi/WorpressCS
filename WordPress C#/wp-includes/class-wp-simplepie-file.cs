using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class HttpService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public HttpService(AppDbContext context)
    {
        _context = context;
        _httpClient = new HttpClient();
    }

    // ارسال درخواست HTTP و ذخیره پاسخ
    public async Task<HttpResponseData> FetchRemoteFileAsync(string url, int timeout = 10, int redirects = 5, Dictionary<string, string> headers = null, string useragent = null)
    {
        var response = new HttpResponseData
        {
            Url = url,
            RequestTime = DateTime.UtcNow,
            Headers = new Dictionary<string, string>()
        };

        try
        {
            // تنظیمات Timeout
            _httpClient.Timeout = TimeSpan.FromSeconds(timeout);

            // اضافه کردن هدرها
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            // تنظیمات User-Agent
            if (!string.IsNullOrEmpty(useragent))
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(useragent);
            }

            // ارسال درخواست
            var httpResponse = await _httpClient.GetAsync(url);

            // ذخیره کد وضعیت
            response.StatusCode = (int)httpResponse.StatusCode;

            // ذخیره هدرها
            foreach (var header in httpResponse.Headers)
            {
                response.Headers[header.Key] = string.Join(", ", header.Value);
            }

            // ذخیره بدنه پاسخ
            response.Body = await httpResponse.Content.ReadAsStringAsync();

            // ذخیره پاسخ در پایگاه داده
            _context.HttpResponses.Add(response);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            response.Body = $"Error: {ex.Message}";
            response.StatusCode = 500; // Internal Server Error
        }

        return response;
    }

    // خواندن اطلاعات پاسخ از پایگاه داده
    public HttpResponseData GetResponseById(int id)
    {
        return _context.HttpResponses.Find(id);
    }

    // خواندن تمام پاسخ‌ها
    public List<HttpResponseData> GetAllResponses()
    {
        return _context.HttpResponses.ToList();
    }
}