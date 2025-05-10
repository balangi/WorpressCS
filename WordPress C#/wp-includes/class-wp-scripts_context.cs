using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Script> Scripts { get; set; }
    public DbSet<ScriptDependency> ScriptDependencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه یک به چند بین Script و ScriptDependency
        modelBuilder.Entity<ScriptDependency>()
            .HasOne(d => d.Script)
            .WithMany(m => m.Dependencies)
            .HasForeignKey(d => d.ScriptId);
    }
}