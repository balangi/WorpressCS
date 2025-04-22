using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Requests.Core.Data;
using Requests.Core.Models;

namespace Requests.Core
{
    /// <summary>
    /// Represents the Requests library for making HTTP requests in C#.
    /// </summary>
    [Obsolete("The Requests class is deprecated. Use WpOrg.Requests instead.")]
    public class Requests
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<Requests> _logger;

        public Requests(AppDbContext dbContext, ILogger<Requests> logger)
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
        [Obsolete("Use WpOrg.Requests.Get() instead.")]
        public async Task<string> GetAsync(string url)
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

                _logger.LogWarning($"Failed to send GET request to {url}: {ex.Message}");
                throw new Exception("Failed to send GET request.", ex);
            }
        }

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the given content.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="content">The content to send in the request body.</param>
        /// <returns>The response content as a string.</returns>
        [Obsolete("Use WpOrg.Requests.Post() instead.")]
        public async Task<string> PostAsync(string url, HttpContent content)
        {
            try
            {
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

                _logger.LogWarning($"Failed to send POST request to {url}: {ex.Message}");
                throw new Exception("Failed to send POST request.", ex);
            }
        }

        /// <summary>
        /// Deprecated autoloader for Requests.
        /// </summary>
        /// <param name="className">The class name to load.</param>
        [Obsolete("Use WpOrg.Requests.Autoload.Load() instead.")]
        public static void Autoloader(string className)
        {
            Console.WriteLine($"Autoloading class: {className} (Deprecated)");
        }

        /// <summary>
        /// Registers the built-in autoloader.
        /// </summary>
        [Obsolete("Include the WpOrg.Requests.Autoload class and call WpOrg.Requests.Autoload.Register() instead.")]
        public static void RegisterAutoloader()
        {
            Console.WriteLine("Registering autoloader (Deprecated)");
        }
    }
}