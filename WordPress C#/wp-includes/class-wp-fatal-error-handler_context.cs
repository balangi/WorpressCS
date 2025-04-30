using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<FatalErrorLog> FatalErrorLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}