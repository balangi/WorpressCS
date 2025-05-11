using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Taxonomy> Taxonomies { get; set; }
    public DbSet<Term> Terms { get; set; }
    public DbSet<TermRelationship> TermRelationships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه بین Taxonomy و Term
        modelBuilder.Entity<Taxonomy>()
            .HasMany(t => t.Terms)
            .WithOne(tm => tm.Taxonomy)
            .HasForeignKey(tm => tm.TaxonomyId);

        // تنظیمات رابطه بین Term و TermRelationship
        modelBuilder.Entity<Term>()
            .HasMany(t => t.TermRelationships)
            .WithOne(tr => tr.Term)
            .HasForeignKey(tr => tr.TermId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}