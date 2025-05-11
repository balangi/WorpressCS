using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Style> Styles { get; set; }
    public DbSet<StyleDependency> StyleDependencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه بین Style و StyleDependency
        modelBuilder.Entity<Style>()
            .HasMany(s => s.Dependencies)
            .WithOne(sd => sd.Style)
            .HasForeignKey(sd => sd.StyleId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}