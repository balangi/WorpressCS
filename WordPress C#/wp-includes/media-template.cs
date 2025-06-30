using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;

public class MediaTemplateService
{
    private readonly ApplicationDbContext _context;

    public MediaTemplateService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Generates an HTML template for an audio player.
    /// </summary>
    public IHtmlContent GenerateAudioTemplate(List<string> audioExtensions, Dictionary<string, string> model)
    {
        var html = new StringBuilder();
        html.AppendLine("<audio style=\"visibility: hidden\" controls class=\"wp-audio-shortcode\">");

        foreach (var ext in audioExtensions)
        {
            if (model.ContainsKey(ext))
            {
                html.AppendLine($"<source src=\"{model[ext]}\" type=\"audio/{ext}\" />");
            }
        }

        html.AppendLine("</audio>");
        return new HtmlString(html.ToString());
    }

    /// <summary>
    /// Generates an HTML template for a video player.
    /// </summary>
    public IHtmlContent GenerateVideoTemplate(List<string> videoExtensions, Dictionary<string, string> model)
    {
        var html = new StringBuilder();
        html.AppendLine("<video class=\"wp-video-shortcode\">");

        foreach (var ext in videoExtensions)
        {
            if (model.ContainsKey(ext))
            {
                html.AppendLine($"<source src=\"{model[ext]}\" type=\"video/{ext}\" />");
            }
        }

        if (model.ContainsKey("poster"))
        {
            html.AppendLine($"<img src=\"{model["poster"]}\" alt=\"Poster\" />");
        }

        html.AppendLine("</video>");
        return new HtmlString(html.ToString());
    }

    /// <summary>
    /// Generates an HTML template for a playlist.
    /// </summary>
    public IHtmlContent GeneratePlaylistTemplate(Playlist playlist)
    {
        var html = new StringBuilder();
        html.AppendLine($"<div class=\"wp-playlist wp-{playlist.Type}\">");

        foreach (var track in playlist.Tracks)
        {
            html.AppendLine("<div class=\"wp-playlist-item\">");
            html.AppendLine($"<a href=\"{track.Src}\">{track.Title}</a>");
            html.AppendLine($"<img src=\"{track.Thumbnail}\" alt=\"Thumbnail\" />");
            html.AppendLine("</div>");
        }

        html.AppendLine("</div>");
        return new HtmlString(html.ToString());
    }

    /// <summary>
    /// Retrieves media attachments based on the provided query.
    /// </summary>
    public List<MediaAttachment> GetMediaAttachments(string fileType = null)
    {
        var query = _context.MediaAttachments.AsQueryable();

        if (!string.IsNullOrEmpty(fileType))
        {
            query = query.Where(m => m.FileType == fileType);
        }

        return query.ToList();
    }
}