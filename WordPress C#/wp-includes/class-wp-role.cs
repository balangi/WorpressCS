using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class Role
{
    /// <summary>
    /// نام نقش
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// لیست دسترسی‌ها
    /// </summary>
    public Dictionary<string, bool> Capabilities { get; set; } = new();

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public Role(string name, Dictionary<string, bool> capabilities)
    {
        Name = name;
        Capabilities = capabilities;
    }
}

/// <summary>
/// سرویس مدیریت نقش‌ها
/// </summary>
public class RoleService
{
    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<RoleService> _logger;

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public RoleService(AppDbContext context, ILogger<RoleService> logger, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// اضافه کردن دسترسی به نقش
    /// </summary>
    public void AddCapability(string roleName, string capability, bool grant = true)
    {
        var role = GetRole(roleName);
        if (role == null)
        {
            throw new ArgumentException($"Role '{roleName}' not found.");
        }

        role.Capabilities[capability] = grant;
        SaveRole(role);

        _logger.LogInformation($"Capability '{capability}' added to role '{roleName}'.");
    }

    /// <summary>
    /// حذف دسترسی از نقش
    /// </summary>
    public void RemoveCapability(string roleName, string capability)
    {
        var role = GetRole(roleName);
        if (role == null)
        {
            throw new ArgumentException($"Role '{roleName}' not found.");
        }

        if (role.Capabilities.ContainsKey(capability))
        {
            role.Capabilities.Remove(capability);
            SaveRole(role);
        }

        _logger.LogInformation($"Capability '{capability}' removed from role '{roleName}'.");
    }

    /// <summary>
    /// بررسی داشتن دسترسی توسط نقش
    /// </summary>
    public bool HasCapability(string roleName, string capability)
    {
        var role = GetRole(roleName);
        if (role == null)
        {
            throw new ArgumentException($"Role '{roleName}' not found.");
        }

        return role.Capabilities.ContainsKey(capability) && role.Capabilities[capability];
    }

    /// <summary>
    /// دریافت نقش
    /// </summary>
    private Role GetRole(string roleName)
    {
        var roles = GetAllRoles();
        return roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// دریافت تمام نقش‌ها
    /// </summary>
    private List<Role> GetAllRoles()
    {
        if (_cache.TryGetValue("roles", out List<Role> cachedRoles))
        {
            return cachedRoles;
        }

        var roles = _context.Roles.ToList();
        _cache.Set("roles", roles, TimeSpan.FromHours(1));
        return roles;
    }

    /// <summary>
    /// ذخیره نقش
    /// </summary>
    private void SaveRole(Role role)
    {
        _context.Roles.Update(role);
        _context.SaveChanges();

        // Update cache
        var roles = GetAllRoles();
        var index = roles.FindIndex(r => r.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase));
        if (index != -1)
        {
            roles[index] = role;
        }
        _cache.Set("roles", roles, TimeSpan.FromHours(1));
    }
}

/// <summary>
/// مدل داده‌ای نقش
/// </summary>
public class RoleEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CapabilitiesJson { get; set; }

    public Role ToRole()
    {
        return new Role(Name, System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(CapabilitiesJson));
    }

    public static RoleEntity FromRole(Role role)
    {
        return new RoleEntity
        {
            Name = role.Name,
            CapabilitiesJson = System.Text.Json.JsonSerializer.Serialize(role.Capabilities)
        };
    }
}

/// <summary>
/// Database Context
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<RoleEntity> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}