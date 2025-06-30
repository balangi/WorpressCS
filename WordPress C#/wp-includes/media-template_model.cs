using System;
using System.Collections.Generic;

public class MediaAttachment
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string FileUrl { get; set; }
    public long FileSize { get; set; }
    public string AltText { get; set; }
    public string Caption { get; set; }
    public string Description { get; set; }
}

public class Playlist
{
    public int Id { get; set; }
    public string Type { get; set; } // "audio" or "video"
    public string Style { get; set; } // "light" or "dark"
    public List<Track> Tracks { get; set; } = new List<Track>();
}

public class Track
{
    public int Id { get; set; }
    public string Src { get; set; }
    public string Title { get; set; }
    public string Thumbnail { get; set; }
}