using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<DuotoneSetting> DuotoneSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}