using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Snoopy.Core.Data;
using Snoopy.Core.Models;

namespace Snoopy.Core
{
    /// <summary>
    /// Represents the Snoopy library for making HTTP requests in C#.
    /// </summary>
    [Obsolete("The Snoopy class is deprecated. Use HttpClient instead.")]
    public class Snoopy
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<Snoopy> _logger;

        // Public variables
        public string Host { get; set; } = "www.php.net";
        public int Port { get; set; } = 80;
        public string ProxyHost { get; set; } = "";
        public int ProxyPort { get; set; } = 0;
        public string ProxyUser { get; set; } = "";
        public string ProxyPass { get; set; } = "";
        public string Agent { get; set; } = "Snoopy v1.2.4";

        public Snoopy(AppDbContext dbContext, ILogger<Snoopy> logger)
        {
            _httpClient = new HttpClient();
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <returns>The response content as a string.</returns>
        [Obsolete("Use HttpClient.GetAsync() instead.")]
        public async Task<string> FetchAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // Log the request
                _dbContext.RequestLogs.Add(new RequestLog
                {
                    Method = "GET",
                    Url = url,
                    Response = content,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = true
                });
                await _dbContext.SaveChangesAsync();

                return content;
            }
            catch (Exception ex)
            {
                // Log the error
                _dbContext.RequestLogs.Add(new RequestLog
                {
                    Method = "GET",
                    Url = url,
                    Response = ex.Message,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                });
                await _dbContext.SaveChangesAsync();

                _logger.LogWarning($"Failed to fetch data from {url}: {ex.Message}");
                throw new Exception("Failed to fetch data.", ex);
            }
        }

        /// <summary>
        /// Sends an HTTP POST request with form data.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="formData">The form data to send.</param>
        /// <returns>The response content as a string.</returns>
        [Obsolete("Use HttpClient.PostAsync() instead.")]
        public async Task<string> SubmitAsync(string url, Dictionary<string, string> formData)
        {
            try
            {
                var content = new FormUrlEncodedContent(formData);
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                // Log the request
                _dbContext.RequestLogs.Add(new RequestLog
                {
                    Method = "POST",
                    Url = url,
                    Response = responseContent,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = true
                });
                await _dbContext.SaveChangesAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                // Log the error
                _dbContext.RequestLogs.Add(new RequestLog
                {
                    Method = "POST",
                    Url = url,
                    Response = ex.Message,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                });
                await _dbContext.SaveChangesAsync();

                _logger.LogWarning($"Failed to submit data to {url}: {ex.Message}");
                throw new Exception("Failed to submit data.", ex);
            }
        }
    }
}