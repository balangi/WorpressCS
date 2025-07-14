using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RevisionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RevisionService> _logger;

    public RevisionService(ApplicationDbContext context, ILogger<RevisionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all revisions for a specific post.
    /// </summary>
    public List<Revision> GetRevisions(int postId)
    {
        var post = _context.Posts.Include(p => p.Revisions).FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            return new List<Revision>();
        }

        return post.Revisions.OrderByDescending(r => r.Modified).ToList();
    }

    /// <summary>
    /// Creates a new revision for a post.
    /// </summary>
    public Revision CreateRevision(int postId, string title, string content, bool isAutosave = false)
    {
        var post = _context.Posts.Find(postId);
        if (post == null)
        {
            throw new InvalidOperationException("Post not found.");
        }

        var revision = new Revision
        {
            PostId = postId,
            Title = title,
            Content = content,
            Modified = DateTime.UtcNow,
            IsAutosave = isAutosave
        };

        _context.Revisions.Add(revision);
        _context.SaveChanges();
        return revision;
    }

    /// <summary>
    /// Restores a post to a specific revision.
    /// </summary>
    public bool RestoreRevision(int revisionId)
    {
        var revision = _context.Revisions.Include(r => r.Post).FirstOrDefault(r => r.Id == revisionId);
        if (revision == null)
        {
            return false;
        }

        var post = revision.Post;
        post.Title = revision.Title;
        post.Content = revision.Content;
        post.Modified = DateTime.UtcNow;

        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Deletes a specific revision.
    /// </summary>
    public bool DeleteRevision(int revisionId)
    {
        var revision = _context.Revisions.Find(revisionId);
        if (revision == null)
        {
            return false;
        }

        _context.Revisions.Remove(revision);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Checks if a revision is an autosave.
    /// </summary>
    public bool IsAutosave(int revisionId)
    {
        var revision = _context.Revisions.Find(revisionId);
        return revision?.IsAutosave ?? false;
    }
}