using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddHttpClient();
services.AddLogging(configure => configure.AddConsole());
services.AddSingleton<OEmbed>();

var serviceProvider = services.BuildServiceProvider();
var oEmbed = serviceProvider.GetRequiredService<OEmbed>();