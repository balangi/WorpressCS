using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Section> Sections { get; set; }
    public DbSet<SectionSetting> SectionSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}