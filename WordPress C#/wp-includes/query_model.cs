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
    public int ParentId { get; set; }
    public Post Parent { get; set; }
    public List<Post> Children { get; set; } = new List<Post>();
}

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}