using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMemoryCache();
services.AddLogging(configure => configure.AddConsole());
services.AddHttpClient();
services.AddSingleton<PluginDependenciesService>();

var serviceProvider = services.BuildServiceProvider();