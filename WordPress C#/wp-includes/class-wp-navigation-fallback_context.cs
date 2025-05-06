public class NavigationDbContext : DbContext
{
    public DbSet<NavigationMenu> NavigationMenus { get; set; }
    public DbSet<ClassicMenu> ClassicMenus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}