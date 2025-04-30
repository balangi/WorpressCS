using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<FeedCacheItem> FeedCacheItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}