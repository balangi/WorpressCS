using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<ScriptModule> ScriptModules { get; set; }
    public DbSet<ScriptModuleDependency> ScriptModuleDependencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه یک به چند بین ScriptModule و ScriptModuleDependency
        modelBuilder.Entity<ScriptModuleDependency>()
            .HasOne(d => d.ScriptModule)
            .WithMany(m => m.Dependencies)
            .HasForeignKey(d => d.ScriptModuleId);
    }
}