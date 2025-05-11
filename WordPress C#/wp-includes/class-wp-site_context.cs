using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Site> Sites { get; set; }
    public DbSet<SiteDetails> SiteDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه یک به یک بین Site و SiteDetails
        modelBuilder.Entity<Site>()
            .HasOne(s => s.Details)
            .WithOne(d => d.Site)
            .HasForeignKey<SiteDetails>(d => d.BlogId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}