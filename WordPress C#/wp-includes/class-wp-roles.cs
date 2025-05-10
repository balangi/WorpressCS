using System.Collections.Generic;
using System.Linq;

public class RoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    // ایجاد نقش جدید
    public void AddRole(string name, string displayName)
    {
        var role = new Role
        {
            Name = name,
            DisplayName = displayName
        };
        _context.Roles.Add(role);
        _context.SaveChanges();
    }

    // افزودن دسترسی به نقش
    public void AddCapabilityToRole(string roleName, string capabilityName)
    {
        var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
        var capability = _context.Capabilities.FirstOrDefault(c => c.Name == capabilityName);

        if (role != null && capability != null)
        {
            var roleCapability = new RoleCapability
            {
                RoleId = role.Id,
                CapabilityId = capability.Id
            };
            _context.RoleCapabilities.Add(roleCapability);
            _context.SaveChanges();
        }
    }

    // دریافت تمام نقش‌ها
    public List<Role> GetAllRoles()
    {
        return _context.Roles.Include(r => r.RoleCapabilities).ThenInclude(rc => rc.Capability).ToList();
    }

    // دریافت دسترسی‌های یک نقش
    public List<string> GetCapabilitiesForRole(string roleName)
    {
        var role = _context.Roles
            .Include(r => r.RoleCapabilities)
            .ThenInclude(rc => rc.Capability)
            .FirstOrDefault(r => r.Name == roleName);

        return role?.RoleCapabilities.Select(rc => rc.Capability.Name).ToList() ?? new List<string>();
    }
}