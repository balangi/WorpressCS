using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Panel> Panels { get; set; }
    public DbSet<PanelSetting> PanelSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}