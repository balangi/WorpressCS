using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<SanitizedData> SanitizedData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}