
    /// <summary>
    /// Database context for EF Core.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CustomSetting> CustomSettings { get; set; }
        public DbSet<CustomSettingOption> CustomSettingOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationship between CustomSetting and CustomSettingOption
            modelBuilder.Entity<CustomSetting>()
                .HasMany(s => s.Options)
                .WithOne(o => o.Setting)
                .HasForeignKey(o => o.SettingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
