using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Taxonomy> Taxonomies { get; set; }
    public DbSet<TaxonomyMeta> TaxonomyMeta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه بین Taxonomy و TaxonomyMeta
        modelBuilder.Entity<Taxonomy>()
            .HasMany(t => t.MetaData)
            .WithOne(tm => tm.Taxonomy)
            .HasForeignKey(tm => tm.TaxonomyId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}