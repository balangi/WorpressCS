using System;
using System.IO;

public class Site
{
    public int BlogId { get; set; }
    public string Domain { get; set; }
    public string Path { get; set; }
    public bool Archived { get; set; }
    public bool Spam { get; set; }
    public bool Deleted { get; set; }
}