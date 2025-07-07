using System;
using System.Collections.Generic;

public class Site
{
    public int BlogId { get; set; }
    public string Domain { get; set; }
    public string Path { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool Public { get; set; }
    public bool Archived { get; set; }
    public bool Mature { get; set; }
    public bool Spam { get; set; }
    public bool Deleted { get; set; }
    public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();
}

public class Network
{
    public int NetworkId { get; set; }
    public string Domain { get; set; }
    public string Path { get; set; }
    public List<Site> Sites { get; set; } = new List<Site>();
}