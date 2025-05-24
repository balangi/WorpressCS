public class ThemeService
{
    public void RegisterDefaultThemeSupports()
    {
        // Default theme features
        AddThemeSupport("title-tag");
        AddThemeSupport("post-thumbnails");
        AddThemeSupport("menus");
        AddThemeSupport("widgets");
    }

    private void AddThemeSupport(string feature)
    {
        Console.WriteLine($"Theme support added: {feature}");
    }
}