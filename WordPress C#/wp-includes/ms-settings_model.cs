using System;

public class Network
{
    public int Id { get; set; }
    public string Domain { get; set; }
    public string Path { get; set; }
    public int BlogId { get; set; }
}

public class Site
{
    public int BlogId { get; set; }
    public string Domain { get; set; }
    public string Path { get; set; }
    public bool Public { get; set; }
    public bool Archived { get; set; }
    public bool Spam { get; set; }
    public bool Deleted { get; set; }
    public int SiteId { get; set; }
}