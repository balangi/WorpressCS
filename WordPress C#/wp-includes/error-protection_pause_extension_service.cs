public class PausedExtensionsService
{
    private readonly PausedExtensionStorage _pluginStorage;
    private readonly PausedExtensionStorage _themeStorage;

    public PausedExtensionsService()
    {
        _pluginStorage = new PausedExtensionStorage();
        _themeStorage = new PausedExtensionStorage();
    }

    public void PausePlugin(string pluginId, object plugin)
    {
        _pluginStorage.Pause(pluginId, plugin);
    }

    public void ResumePlugin(string pluginId)
    {
        _pluginStorage.Resume(pluginId);
    }

    public void PauseTheme(string themeId, object theme)
    {
        _themeStorage.Pause(themeId, theme);
    }

    public void ResumeTheme(string themeId)
    {
        _themeStorage.Resume(themeId);
    }

    public bool IsPluginPaused(string pluginId)
    {
        return _pluginStorage.IsPaused(pluginId);
    }

    public bool IsThemePaused(string themeId)
    {
        return _themeStorage.IsPaused(themeId);
    }
}