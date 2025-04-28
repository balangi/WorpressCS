using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Widget> Widgets { get; set; }
    public DbSet<WidgetSetting> WidgetSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}