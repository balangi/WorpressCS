using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

public class MediaService
{
    private readonly ApplicationDbContext _context;

    public MediaService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves media attachments for a specific post.
    /// </summary>
    public List<MediaAttachment> GetAttachedMedia(int postId, string mimeType = null)
    {
        var query = _context.MediaAttachments.Where(m => m.PostId == postId);

        if (!string.IsNullOrEmpty(mimeType))
        {
            query = query.Where(m => m.FileType == mimeType);
        }

        return query.ToList();
    }

    /// <summary>
    /// Creates a playlist from media attachments.
    /// </summary>
    public Playlist CreatePlaylist(string type, string style, List<int> attachmentIds)
    {
        var attachments = _context.MediaAttachments.Where(m => attachmentIds.Contains(m.Id)).ToList();

        var tracks = attachments.Select(a => new Track
        {
            Src = a.FileUrl,
            Title = a.FileName,
            Thumbnail = GenerateThumbnail(a.FileUrl)
        }).ToList();

        var playlist = new Playlist
        {
            Type = type,
            Style = style,
            Tracks = tracks
        };

        _context.Playlists.Add(playlist);
        _context.SaveChanges();

        return playlist;
    }

    /// <summary>
    /// Generates a thumbnail URL for a media file.
    /// </summary>
    private string GenerateThumbnail(string fileUrl)
    {
        // Placeholder logic for generating thumbnails
        return fileUrl + "?thumbnail=true";
    }

    /// <summary>
    /// Searches for media items based on a query.
    /// </summary>
    public List<MediaAttachment> SearchMedia(string query)
    {
        return _context.MediaAttachments
            .Where(m => m.FileName.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Handles media upload and stores the file in the database.
    /// </summary>
    public MediaAttachment UploadMedia(IFormFile file, int postId)
    {
        var fileName = file.FileName;
        var fileType = file.ContentType;
        var fileSize = file.Length;

        // Save the file to a storage location (e.g., disk or cloud)
        var fileUrl = SaveFileToStorage(file);

        var mediaAttachment = new MediaAttachment
        {
            FileName = fileName,
            FileType = fileType,
            FileSize = fileSize,
            FileUrl = fileUrl,
            PostId = postId
        };

        _context.MediaAttachments.Add(mediaAttachment);
        _context.SaveChanges();

        return mediaAttachment;
    }

    /// <summary>
    /// Saves a file to storage and returns its URL.
    /// </summary>
    private string SaveFileToStorage(IFormFile file)
    {
        // Placeholder logic for saving files to storage
        return $"/uploads/{file.FileName}";
    }
}