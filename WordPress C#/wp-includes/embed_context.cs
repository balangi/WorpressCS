public class ApplicationDbContext : DbContext
{
    public DbSet<EmbedData> Embeds { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}