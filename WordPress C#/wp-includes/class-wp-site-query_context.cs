using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Site> Sites { get; set; }
    public DbSet<SiteMeta> SiteMeta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه بین Site و SiteMeta
        modelBuilder.Entity<Site>()
            .HasMany(s => s.MetaData)
            .WithOne(sm => sm.Site)
            .HasForeignKey(sm => sm.SiteId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}