using Microsoft.EntityFrameworkCore;

namespace WordPress.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<BlockMetadataCollection> BlockMetadataCollections { get; set; }
        public DbSet<BlockMetadataFile> BlockMetadataFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}