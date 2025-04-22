using Microsoft.EntityFrameworkCore;

namespace Requests.Core.Data
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