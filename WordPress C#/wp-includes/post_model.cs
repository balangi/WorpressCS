using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Excerpt { get; set; }
    public string Status { get; set; } // e.g., 'publish', 'draft'
    public string Type { get; set; } // e.g., 'post', 'page'
    public DateTime Date { get; set; }
    public DateTime Modified { get; set; }
    public string AuthorId { get; set; }
    public User Author { get; set; }
    public int ParentId { get; set; }
    public Post Parent { get; set; }
    public List<Post> Children { get; set; } = new List<Post>();
    public string Password { get; set; }
    public string Name { get; set; }
    public string Mime { get; set; }
    public string Guid { get; set; }
    public int MenuOrder { get; set; }
    public string CommentStatus { get; set; }
    public string PingStatus { get; set; }
    public string ToPing { get; set; }
    public string Pinged { get; set; }
}

public class PostType
{
    public string Name { get; set; }
    public string SingularName { get; set; }
    public bool Public { get; set; }
    public bool Hierarchical { get; set; }
    public bool HasArchive { get; set; }
    public string RewriteSlug { get; set; }
    public Dictionary<string, string> Capabilities { get; set; } = new Dictionary<string, string>();
    public List<string> Supports { get; set; } = new List<string>();
}

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}