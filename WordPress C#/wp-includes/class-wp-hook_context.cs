using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Hook> Hooks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}