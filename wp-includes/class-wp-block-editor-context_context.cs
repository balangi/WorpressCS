using Microsoft.EntityFrameworkCore;

namespace WordPress.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<BlockEditorContext> BlockEditorContexts { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}