using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<TextDomain> TextDomains { get; set; }
    public DbSet<Locale> Locales { get; set; }
    public DbSet<TranslationFile> TranslationFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تنظیمات رابطه بین TextDomain و Locale
        modelBuilder.Entity<TextDomain>()
            .HasMany(td => td.Locales)
            .WithOne(l => l.TextDomain)
            .HasForeignKey(l => l.TextDomainId);

        // تنظیمات رابطه بین Locale و TranslationFile
        modelBuilder.Entity<Locale>()
            .HasMany(l => l.TranslationFiles)
            .WithOne(tf => tf.Locale)
            .HasForeignKey(tf => tf.LocaleId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionStringHere");
    }
}