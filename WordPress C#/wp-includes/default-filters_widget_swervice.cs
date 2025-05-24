public class WidgetService
{
    private readonly List<IWidget> _widgets = new();

    public void RegisterWidgets()
    {
        Register(new RecentPostsWidget());
        Register(new SearchWidget());
        Register(new CustomMenuWidget());
    }

    public void Register(IWidget widget)
    {
        _widgets.Add(widget);
        Console.WriteLine($"Widget registered: {widget.Name}");
    }

    public IEnumerable<IWidget> GetWidgets() => _widgets;
}