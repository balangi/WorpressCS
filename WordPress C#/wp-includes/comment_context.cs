public class ApplicationDbContext : DbContext
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentMeta> CommentMetas { get; set; }
    public DbSet<Post> Posts { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}