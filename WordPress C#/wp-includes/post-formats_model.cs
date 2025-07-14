using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Format { get; set; } // e.g., 'standard', 'aside', 'gallery'
    public DateTime Date { get; set; }
    public DateTime Modified { get; set; }
    public string AuthorId { get; set; }
    public User Author { get; set; }
}

public class PostFormat
{
    public string Slug { get; set; }
    public string DisplayName { get; set; }
}