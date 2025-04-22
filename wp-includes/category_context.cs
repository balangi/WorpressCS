using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("wp_terms");
            entity.Property(e => e.TermId).HasColumnName("term_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Parent).HasColumnName("parent");
            entity.Property(e => e.Count).HasColumnName("count");
        });
    }
}