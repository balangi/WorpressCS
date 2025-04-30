using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<EmbedSetting> EmbedSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}