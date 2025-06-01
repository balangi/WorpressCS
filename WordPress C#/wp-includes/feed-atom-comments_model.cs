using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedDate { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public class Comment
{
    public int Id { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
    public string AuthorUrl { get; set; }
    public DateTime Date { get; set; }
    public int ParentCommentId { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}