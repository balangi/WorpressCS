using System;
using System.Collections.Generic;

public class Template
{
    public string Slug { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
}

public class FeedLink
{
    public string Type { get; set; }
    public string Url { get; set; }
}

public class PaginationOptions
{
    public string BaseUrl { get; set; }
    public string Format { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public bool ShowAll { get; set; }
    public bool PrevNext { get; set; }
    public string PrevText { get; set; }
    public string NextText { get; set; }
}