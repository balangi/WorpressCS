using System.Collections.Generic;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public ICollection<RoleCapability> RoleCapabilities { get; set; }
}

public class Capability
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<RoleCapability> RoleCapabilities { get; set; }
}

public class RoleCapability
{
    public int RoleId { get; set; }
    public Role Role { get; set; }

    public int CapabilityId { get; set; }
    public Capability Capability { get; set; }
}