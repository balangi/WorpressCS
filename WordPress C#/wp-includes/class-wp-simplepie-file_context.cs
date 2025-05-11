using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<HttpResponseData> HttpResponses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}