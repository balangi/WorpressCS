var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IWidgetService, WidgetService>();

var app = builder.Build();

// Register default widgets
using (var scope = app.Services.CreateScope())
{
    var widgetService = scope.ServiceProvider.GetRequiredService<IWidgetService>();
    widgetService.RegisterWidgets();
}

app.MapGet("/", () =>
{
    using var scope = app.Services.CreateScope();
    var widgetService = scope.ServiceProvider.GetRequiredService<IWidgetService>();

    var output = new StringBuilder();

    foreach (var widget in widgetService.GetRegisteredWidgets())
    {
        output.AppendLine($"<h3>{widget.Name}</h3>");
        var htmlHelper = A.Fake<IHtmlHelper>(); // یا از HtmlHelper واقعی استفاده کنید
        widget.Widget(htmlHelper, new Dictionary<string, object>(), new Dictionary<string, object>());
    }

    return output.ToString();
});

app.Run();