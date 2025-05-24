public class WidgetService : IWidgetService
{
    private readonly List<Widget> _widgets = new();

    public void RegisterWidgets()
    {
        Register(new PagesWidget());
        Register(new SearchWidget());
        Register(new RecentPostsWidget());
        // بقیه ویجت‌ها را اینجا اضافه کنید
    }

    public void Register(Widget widget)
    {
        _widgets.Add(widget);
    }

    public List<Widget> GetRegisteredWidgets() => _widgets;

    public Widget GetWidget(string idBase) => _widgets.FirstOrDefault(w => w.IdBase == idBase);
}