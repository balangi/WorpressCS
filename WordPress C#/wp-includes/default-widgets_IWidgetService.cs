public interface IWidgetService
{
    void RegisterWidgets();
    List<Widget> GetRegisteredWidgets();
    Widget GetWidget(string idBase);
}