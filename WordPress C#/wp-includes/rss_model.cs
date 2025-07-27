using System;
using System.Collections.Generic;

public class RssFeed
{
    public string Title { get; set; }
    public string Link { get; set; }
    public string Description { get; set; }
    public List<RssItem> Items { get; set; } = new List<RssItem>();
}

public class RssItem
{
    public string Title { get; set; }
    public string Link { get; set; }
    public string Description { get; set; }
    public DateTime PublishDate { get; set; }
}