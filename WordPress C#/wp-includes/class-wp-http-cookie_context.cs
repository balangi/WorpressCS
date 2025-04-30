using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<HttpCookieModel> HttpCookies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}