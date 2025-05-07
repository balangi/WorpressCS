using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMemoryCache();
services.AddLogging(configure => configure.AddConsole());
services.AddSingleton<ObjectCache>();

var serviceProvider = services.BuildServiceProvider();
var objectCache = serviceProvider.GetRequiredService<ObjectCache>();