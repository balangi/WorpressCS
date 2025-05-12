public class WidgetFactory : IWidgetFactory
{
    private readonly Dictionary<string, IWidget> _widgets = new();
    private readonly IServiceProvider _serviceProvider;

    public WidgetFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Register<T>() where T : class, IWidget
    {
        var widget = _serviceProvider.GetRequiredService<T>();
        var idBase = GetIdBase<T>();

        if (!_widgets.ContainsKey(idBase))
        {
            _widgets[idBase] = widget;
        }
    }

    public void Register(IWidget widget)
    {
        var idBase = widget.IdBase;
        if (!_widgets.ContainsKey(idBase))
        {
            _widgets[idBase] = widget;
        }
    }

    public void Unregister(string idBase)
    {
        if (_widgets.ContainsKey(idBase))
        {
            _widgets.Remove(idBase);
        }
    }

    public IWidget GetWidget(string idBase)
    {
        if (_widgets.TryGetValue(idBase, out var widget))
        {
            return widget;
        }

        return null;
    }

    public IEnumerable<IWidget> GetAllWidgets()
    {
        return _widgets.Values.ToList();
    }

    private static string GetIdBase<T>() where T : class, IWidget
    {
        var type = typeof(T);
        var name = type.Name.Replace("Widget", "").ToLowerInvariant();
        return name;
    }
}