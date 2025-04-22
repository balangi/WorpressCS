using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public bool IsSuperAdmin { get; set; }
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Capability> Capabilities { get; set; } = new List<Capability>();
}

public class Capability
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string PostType { get; set; }
    public string Status { get; set; }
    public User Author { get; set; }
}