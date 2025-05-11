using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Term> Terms { get; set; }
    public DbSet<TermTaxonomy> TermTaxonomies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه بین Term و TermTaxonomy
        modelBuilder.Entity<Term>()
            .HasOne(t => t.TermTaxonomy)
            .WithOne(tt => tt.Term)
            .HasForeignKey<TermTaxonomy>(tt => tt.TermId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}