using Microsoft.EntityFrameworkCore;

namespace PHPMailer.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<SmtpLog> SmtpLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }
    }
}