public interface IWidgetFactory
{
    void Register<T>() where T : class, IWidget;
    void Register(IWidget widget);
    void Unregister(string idBase);
    IWidget GetWidget(string idBase);
    IEnumerable<IWidget> GetAllWidgets();
}