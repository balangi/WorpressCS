using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class PostService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostService> _logger;

    public PostService(ApplicationDbContext context, ILogger<PostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a post by its ID.
    /// </summary>
    public Post GetPost(int postId)
    {
        return _context.Posts.Include(p => p.Author).Include(p => p.Children).FirstOrDefault(p => p.Id == postId);
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
    /// Creates a new post.
    /// </summary>
    public Post CreatePost(Post post)
    {
        if (string.IsNullOrEmpty(post.Title))
        {
            throw new ArgumentException("Post title cannot be empty.");
        }

        post.Date = DateTime.UtcNow;
        post.Modified = DateTime.UtcNow;

        _context.Posts.Add(post);
        _context.SaveChanges();
        return post;
    }

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    public Post UpdatePost(int postId, Post updatedPost)
    {
        var existingPost = _context.Posts.Find(postId);
        if (existingPost == null)
        {
            throw new InvalidOperationException("Post not found.");
        }

        existingPost.Title = updatedPost.Title;
        existingPost.Content = updatedPost.Content;
        existingPost.Excerpt = updatedPost.Excerpt;
        existingPost.Status = updatedPost.Status;
        existingPost.Modified = DateTime.UtcNow;

        _context.SaveChanges();
        return existingPost;
    }

    /// <summary>
    /// Deletes a post.
    /// </summary>
    public bool DeletePost(int postId)
    {
        var post = _context.Posts.Find(postId);
        if (post == null)
        {
            return false;
        }

        _context.Posts.Remove(post);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Registers a new post type.
    /// </summary>
    public void RegisterPostType(PostType postType)
    {
        if (string.IsNullOrEmpty(postType.Name))
        {
            throw new ArgumentException("Post type name cannot be empty.");
        }

        if (_context.PostTypes.Any(pt => pt.Name == postType.Name))
        {
            throw new InvalidOperationException($"Post type '{postType.Name}' already exists.");
        }

        _context.PostTypes.Add(postType);
        _context.SaveChanges();
    }

    /// <summary>
    /// Retrieves all registered post types.
    /// </summary>
    public List<PostType> GetAllPostTypes()
    {
        return _context.PostTypes.ToList();
    }

    /// <summary>
    /// Checks if a user has the required capability for a post.
    /// </summary>
    public bool CheckCapability(string userId, string capability, int postId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        var post = _context.Posts.Find(postId);

        if (user == null || post == null)
        {
            return false;
        }

        var postType = _context.PostTypes.FirstOrDefault(pt => pt.Name == post.Type);
        if (postType == null)
        {
            return false;
        }

        return postType.Capabilities.ContainsKey(capability) && postType.Capabilities[capability] == user.Role;
    }
}