public class ApplicationDbContext : DbContext
{
    public DbSet<ErrorLog> ErrorLogs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}