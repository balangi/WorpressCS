using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<HttpRequestLog> HttpRequestLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}