public class ApplicationDbContext : DbContext
{
    public DbSet<DeprecatedFunction> DeprecatedFunctions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}