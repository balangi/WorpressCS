public class WP_Theme
{
    public string Name { get; set; }
    public string Stylesheet { get; set; }
    public string ThemeRoot { get; set; }

    public WP_Theme ParentTheme { get; set; }

    public WP_Theme(string name, string themeRoot, WP_Theme parent = null)
    {
        Name = name;
        Stylesheet = name;
        ThemeRoot = themeRoot;
        ParentTheme = parent;
    }

    public WP_Theme Parent()
    {
        return ParentTheme;
    }

    public string GetFilePath(string fileName)
    {
        return Path.Combine(ThemeRoot, Stylesheet, fileName);
    }
}