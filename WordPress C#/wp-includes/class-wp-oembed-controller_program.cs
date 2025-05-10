var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddLogging(configure => configure.AddConsole());

var app = builder.Build();
app.MapControllers();
app.Run();