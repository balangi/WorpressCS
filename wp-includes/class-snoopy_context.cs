using Microsoft.EntityFrameworkCore;

namespace Snoopy.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<RequestLog> RequestLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}