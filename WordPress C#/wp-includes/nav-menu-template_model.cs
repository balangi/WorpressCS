using System;
using System.Collections.Generic;

public class NavMenu
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<NavItem> Items { get; set; } = new List<NavItem>();
}

public class NavItem
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public NavMenu Menu { get; set; }

    public string Title { get; set; }
    public string Url { get; set; }
    public string Target { get; set; }
    public string Description { get; set; }
    public int ParentItemId { get; set; }
    public NavItem ParentItem { get; set; }
    public List<NavItem> ChildItems { get; set; } = new List<NavItem>();

    public string ObjectType { get; set; } // e.g., 'post_type', 'taxonomy'
    public int ObjectId { get; set; } // ID of the linked object (e.g., post ID, term ID)
}