using Microsoft.EntityFrameworkCore;

namespace POP3.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pop3Log> Pop3Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}