using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class PostFormatService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostFormatService> _logger;

    public PostFormatService(ApplicationDbContext context, ILogger<PostFormatService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the format slug for a post.
    /// </summary>
    public string GetPostFormat(int? postId = null)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            return null;
        }

        return post.Format ?? "standard";
    }

    /// <summary>
    /// Checks if a post has any of the given formats.
    /// </summary>
    public bool HasPostFormat(List<string> formats, int? postId = null)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            return false;
        }

        return formats.Contains(post.Format);
    }

    /// <summary>
    /// Assigns a format to a post.
    /// </summary>
    public bool SetPostFormat(int postId, string format)
    {
        var post = _context.Posts.Find(postId);
        if (post == null)
        {
            _logger.LogError($"Post with ID {postId} not found.");
            return false;
        }

        if (!IsValidFormat(format))
        {
            format = "standard";
        }

        post.Format = format;
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Retrieves all post format slugs and their display names.
    /// </summary>
    public Dictionary<string, string> GetPostFormatStrings()
    {
        return new Dictionary<string, string>
        {
            { "standard", "Standard" },
            { "aside", "Aside" },
            { "chat", "Chat" },
            { "gallery", "Gallery" },
            { "link", "Link" },
            { "image", "Image" },
            { "quote", "Quote" },
            { "status", "Status" },
            { "video", "Video" },
            { "audio", "Audio" }
        };
    }

    /// <summary>
    /// Retrieves the translated display name for a post format slug.
    /// </summary>
    public string GetPostFormatString(string slug)
    {
        var formats = GetPostFormatStrings();
        return formats.ContainsKey(slug) ? formats[slug] : formats["standard"];
    }

    /// <summary>
    /// Retrieves the link to a post format index.
    /// </summary>
    public string GetPostFormatLink(string format)
    {
        var formats = GetPostFormatStrings();
        if (!formats.ContainsKey(format))
        {
            return null;
        }

        return $"/format/{format}";
    }

    /// <summary>
    /// Validates if the given format is valid.
    /// </summary>
    private bool IsValidFormat(string format)
    {
        var formats = GetPostFormatStrings().Keys;
        return formats.Contains(format);
    }
}