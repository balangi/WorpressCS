using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class PostThumbnailService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostThumbnailService> _logger;

    public PostThumbnailService(ApplicationDbContext context, ILogger<PostThumbnailService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Determines whether a post has a thumbnail attached.
    /// </summary>
    public bool HasThumbnail(int postId)
    {
        var post = _context.Posts.Include(p => p.Thumbnail).FirstOrDefault(p => p.Id == postId);
        return post != null && post.ThumbnailId > 0;
    }

    /// <summary>
    /// Retrieves the thumbnail URL for a post.
    /// </summary>
    public string GetThumbnailUrl(int postId)
    {
        var post = _context.Posts.Include(p => p.Thumbnail).FirstOrDefault(p => p.Id == postId);
        if (post == null || post.Thumbnail == null)
        {
            return null;
        }

        return post.Thumbnail.Url;
    }

    /// <summary>
    /// Sets the thumbnail for a post.
    /// </summary>
    public bool SetThumbnail(int postId, int attachmentId)
    {
        var post = _context.Posts.Find(postId);
        var attachment = _context.Attachments.Find(attachmentId);

        if (post == null || attachment == null)
        {
            _logger.LogError($"Post or attachment not found. Post ID: {postId}, Attachment ID: {attachmentId}");
            return false;
        }

        post.ThumbnailId = attachmentId;
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Removes the thumbnail from a post.
    /// </summary>
    public bool RemoveThumbnail(int postId)
    {
        var post = _context.Posts.Find(postId);
        if (post == null)
        {
            _logger.LogError($"Post not found. Post ID: {postId}");
            return false;
        }

        post.ThumbnailId = 0;
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Checks if the current theme supports post thumbnails.
    /// </summary>
    public bool IsThumbnailSupported(string postType)
    {
        // Simulate theme support check
        return postType switch
        {
            "post" => true,
            "page" => true,
            _ => false
        };
    }
}