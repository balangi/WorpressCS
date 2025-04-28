using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<CustomSetting> CustomSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}