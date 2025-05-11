using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Theme> Themes { get; set; }
    public DbSet<ThemeHeader> ThemeHeaders { get; set; }
    public DbSet<ThemeTemplate> ThemeTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه بین Theme و ThemeHeader
        modelBuilder.Entity<Theme>()
            .HasMany(t => t.Headers)
            .WithOne(th => th.Theme)
            .HasForeignKey(th => th.ThemeId);

        // تنظیمات رابطه بین Theme و ThemeTemplate
        modelBuilder.Entity<Theme>()
            .HasMany(t => t.Templates)
            .WithOne(tt => tt.Theme)
            .HasForeignKey(tt => tt.ThemeId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}