using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddDbContext<RecoveryDbContext>(options =>
    options.UseSqlServer("YourConnectionStringHere"));
services.AddLogging(configure => configure.AddConsole());
services.AddSingleton<PausedExtensionsStorage>();

var serviceProvider = services.BuildServiceProvider();