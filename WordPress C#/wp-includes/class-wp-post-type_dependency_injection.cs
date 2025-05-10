using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMemoryCache();
services.AddLogging(configure => configure.AddConsole());
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("YourConnectionStringHere"));
services.AddSingleton<PostType>();

var serviceProvider = services.BuildServiceProvider();