using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class QueryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QueryService> _logger;

    public QueryService(ApplicationDbContext context, ILogger<QueryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves posts based on query parameters.
    /// </summary>
    public List<Post> GetPosts(string postType = "any", string status = "any", int? authorId = null, DateTime? beforeDate = null, DateTime? afterDate = null)
    {
        var query = _context.Posts.AsQueryable();

        if (postType != "any")
        {
            query = query.Where(p => p.Type == postType);
        }

        if (status != "any")
        {
            query = query.Where(p => p.Status == status);
        }

        if (authorId.HasValue)
        {
            query = query.Where(p => p.AuthorId == authorId.ToString());
        }

        if (beforeDate.HasValue)
        {
            query = query.Where(p => p.Date < beforeDate.Value);
        }

        if (afterDate.HasValue)
        {
            query = query.Where(p => p.Date > afterDate.Value);
        }

        return query.ToList();
    }

    /// <summary>
    /// Retrieves a single post by ID.
    /// </summary>
    public Post GetPostById(int postId)
    {
        return _context.Posts.FirstOrDefault(p => p.Id == postId);
    }

    /// <summary>
    /// Retrieves posts by author.
    /// </summary>
    public List<Post> GetPostsByAuthor(string authorId)
    {
        return _context.Posts.Where(p => p.AuthorId == authorId).ToList();
    }

    /// <summary>
    /// Retrieves posts by date range.
    /// </summary>
    public List<Post> GetPostsByDateRange(DateTime startDate, DateTime endDate)
    {
        return _context.Posts.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
    }

    /// <summary>
    /// Searches for posts by title or content.
    /// </summary>
    public List<Post> SearchPosts(string searchTerm)
    {
        return _context.Posts
            .Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm))
            .ToList();
    }

    /// <summary>
    /// Paginates the query results.
    /// </summary>
    public List<Post> PaginatePosts(int pageNumber, int pageSize)
    {
        return _context.Posts
            .OrderByDescending(p => p.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}