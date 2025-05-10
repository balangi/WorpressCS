using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<SessionToken> SessionTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}