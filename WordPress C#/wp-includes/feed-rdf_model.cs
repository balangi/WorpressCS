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
    public string Author { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}