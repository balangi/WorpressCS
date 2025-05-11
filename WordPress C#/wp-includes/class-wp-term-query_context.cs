using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Term> Terms { get; set; }
    public DbSet<TermTaxonomy> TermTaxonomies { get; set; }
    public DbSet<TermMeta> TermMeta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه بین Term و TermTaxonomy
        modelBuilder.Entity<Term>()
            .HasMany(t => t.Taxonomies)
            .WithOne(tt => tt.Term)
            .HasForeignKey(tt => tt.TermId);

        // تنظیمات رابطه بین Term و TermMeta
        modelBuilder.Entity<Term>()
            .HasMany(t => t.MetaData)
            .WithOne(tm => tm.Term)
            .HasForeignKey(tm => tm.TermId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}