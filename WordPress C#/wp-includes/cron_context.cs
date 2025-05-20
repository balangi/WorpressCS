public class ApplicationDbContext : DbContext
{
    public DbSet<CronJob> CronJobs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}