using Microsoft.EntityFrameworkCore;

public class LocalizationDbContext : DbContext
{
    public DbSet<Translation> Translations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}