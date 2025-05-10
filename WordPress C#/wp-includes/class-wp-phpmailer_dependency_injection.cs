using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLocalization(options => options.ResourcesPath = "Resources");
services.AddSingleton<WpPhpMailer>();

var serviceProvider = services.BuildServiceProvider();