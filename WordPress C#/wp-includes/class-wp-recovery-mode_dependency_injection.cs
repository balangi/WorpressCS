using Microsoft.Extensions.DependencyInjection;

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