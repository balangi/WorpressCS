using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<Capability> Capabilities { get; set; }
    public DbSet<RoleCapability> RoleCapabilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه چند به چند بین Role و Capability
        modelBuilder.Entity<RoleCapability>()
            .HasKey(rc => new { rc.RoleId, rc.CapabilityId });

        modelBuilder.Entity<RoleCapability>()
            .HasOne(rc => rc.Role)
            .WithMany(r => r.RoleCapabilities)
            .HasForeignKey(rc => rc.RoleId);

        modelBuilder.Entity<RoleCapability>()
            .HasOne(rc => rc.Capability)
            .WithMany(c => c.RoleCapabilities)
            .HasForeignKey(rc => rc.CapabilityId);
    }
}