using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Excerpt { get; set; }
    public DateTime PublishedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string Permalink { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Comment
{
    public int Id { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public string Permalink { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}