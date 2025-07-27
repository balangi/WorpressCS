using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<RewriteRule> RewriteRules { get; set; }
    public DbSet<Endpoint> Endpoints { get; set; }
    public DbSet<Permastruct> Permastructs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}