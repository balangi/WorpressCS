using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RestApiService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RestApiService> _logger;

    public RestApiService(ApplicationDbContext context, ILogger<RestApiService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Registers REST API routes.
    /// </summary>
    public void RegisterRestRoutes()
    {
        // Add REST-specific routes here
        Console.WriteLine("REST API routes registered.");
    }

    /// <summary>
    /// Handles REST API requests.
    /// </summary>
    public RestResponse HandleRestRequest(string path, string method, IDictionary<string, string> queryParams)
    {
        var response = new RestResponse();

        try
        {
            // Simulate route matching and processing
            var route = _context.RestRoutes.FirstOrDefault(r => r.Path == path);
            if (route == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Data = new { Message = "Route not found." };
                return response;
            }

            // Process the request based on the HTTP method
            switch (method.ToUpper())
            {
                case "GET":
                    response.Data = GetData(path, queryParams);
                    break;
                case "POST":
                    response.Data = PostData(path, queryParams);
                    break;
                case "PUT":
                    response.Data = UpdateData(path, queryParams);
                    break;
                case "DELETE":
                    response.Data = DeleteData(path, queryParams);
                    break;
                default:
                    response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    response.Data = new { Message = "Method not allowed." };
                    return response;
            }

            response.StatusCode = StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling REST request.");
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Data = new { Message = "Internal server error." };
        }

        return response;
    }

    /// <summary>
    /// Retrieves data for a GET request.
    /// </summary>
    private object GetData(string path, IDictionary<string, string> queryParams)
    {
        // Simulate fetching data from the database
        var data = _context.Posts.ToList();
        return data;
    }

    /// <summary>
    /// Creates data for a POST request.
    /// </summary>
    private object PostData(string path, IDictionary<string, string> queryParams)
    {
        // Simulate creating data in the database
        var post = new Post
        {
            Title = queryParams["title"],
            Content = queryParams["content"],
            Date = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        _context.SaveChanges();
        return new { Message = "Post created successfully.", PostId = post.Id };
    }

    /// <summary>
    /// Updates data for a PUT request.
    /// </summary>
    private object UpdateData(string path, IDictionary<string, string> queryParams)
    {
        var postId = int.Parse(queryParams["id"]);
        var post = _context.Posts.Find(postId);

        if (post == null)
        {
            return new { Message = "Post not found." };
        }

        post.Title = queryParams["title"];
        post.Content = queryParams["content"];
        _context.SaveChanges();

        return new { Message = "Post updated successfully." };
    }

    /// <summary>
    /// Deletes data for a DELETE request.
    /// </summary>
    private object DeleteData(string path, IDictionary<string, string> queryParams)
    {
        var postId = int.Parse(queryParams["id"]);
        var post = _context.Posts.Find(postId);

        if (post == null)
        {
            return new { Message = "Post not found." };
        }

        _context.Posts.Remove(post);
        _context.SaveChanges();

        return new { Message = "Post deleted successfully." };
    }
}