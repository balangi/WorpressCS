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
    public List<Revision> Revisions { get; set; } = new List<Revision>();
}

public class Revision
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Modified { get; set; }
    public bool IsAutosave { get; set; }
}