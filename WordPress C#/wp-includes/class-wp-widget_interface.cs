public interface IWidgetService
{
    Task<List<WidgetSettings>> GetWidgetSettingsAsync(string widgetIdBase);
    Task<Dictionary<string, object>> GetSettingsAsync(string widgetId);
    Task SaveSettingsAsync(string widgetId, Dictionary<string, object> settings);
}