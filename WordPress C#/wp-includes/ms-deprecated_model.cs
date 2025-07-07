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

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsSuperAdmin { get; set; }
    public List<Role> Roles { get; set; } = new List<Role>();
}

public class Role
{
    public int RoleId { get; set; }
    public string Name { get; set; }
    public List<User> Users { get; set; } = new List<User>();
}