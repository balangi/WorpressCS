using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<EditorSetting> EditorSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}