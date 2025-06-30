using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Permalink { get; set; }
    public string ShortLink { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
}

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}

public class Term
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Taxonomy { get; set; }
    public string FeedLink { get; set; }
}

public class SiteSettings
{
    public string HomeUrl { get; set; }
    public string AdminUrl { get; set; }
}