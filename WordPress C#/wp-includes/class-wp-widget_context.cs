public class ApplicationDbContext : DbContext
{
    public DbSet<WidgetSettings> WidgetSettings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}