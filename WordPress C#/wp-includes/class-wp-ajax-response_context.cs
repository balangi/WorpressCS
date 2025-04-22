using Microsoft.EntityFrameworkCore;

namespace WordPress.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AjaxResponse> AjaxResponses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}