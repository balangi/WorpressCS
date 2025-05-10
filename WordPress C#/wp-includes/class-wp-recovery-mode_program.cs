using System;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddLogging(configure => configure.AddConsole());
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer("YourConnectionStringHere"));
        services.AddSingleton<RecoveryModeService>();
        services.AddSingleton<RecoveryModeCookieService>();
        services.AddSingleton<RecoveryModeKeyService>();
        services.AddSingleton<RecoveryModeLinkService>();
        services.AddSingleton<RecoveryModeEmailService>();

        var serviceProvider = services.BuildServiceProvider();
        var recoveryModeService = serviceProvider.GetRequiredService<RecoveryModeService>();

        // مقداردهی اولیه
        recoveryModeService.Initialize();

        // بررسی فعال بودن حالت بازیابی
        if (recoveryModeService.IsActive())
        {
            Console.WriteLine("Recovery mode is active.");
        }
        else
        {
            Console.WriteLine("Recovery mode is not active.");
        }

        // خروج از حالت بازیابی
        if (recoveryModeService.ExitRecoveryMode())
        {
            Console.WriteLine("Exited recovery mode successfully.");
        }
        else
        {
            Console.WriteLine("Failed to exit recovery mode.");
        }
    }
}