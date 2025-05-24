var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<WidgetService>();

var app = builder.Build();

// Configure middleware
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Register default filters/actions
RegisterDefaultFilters(app.Services);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void RegisterDefaultFilters(IServiceProvider serviceProvider)
{
    var themeService = serviceProvider.GetRequiredService<ThemeService>();
    themeService.RegisterDefaultThemeSupports();

    var widgetService = serviceProvider.GetRequiredService<WidgetService>();
    widgetService.RegisterWidgets();
}