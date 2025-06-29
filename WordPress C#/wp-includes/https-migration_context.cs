using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<SiteSettings> SiteSettings { get; set; }
    public DbSet<Option> Options { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}

public class Option
{
    public string Name { get; set; }
    public string Value { get; set; }
}