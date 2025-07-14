using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Status { get; set; } // e.g., 'publish', 'draft'
    public string Type { get; set; } // e.g., 'post', 'page'
    public DateTime Date { get; set; }
    public DateTime Modified { get; set; }
    public string AuthorId { get; set; }
    public User Author { get; set; }
    public int ThumbnailId { get; set; }
    public Attachment Thumbnail { get; set; }
}

public class Attachment
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string AltText { get; set; }
    public string Caption { get; set; }
    public string Description { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}