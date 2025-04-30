using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Error> Errors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}