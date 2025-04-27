public class Panel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; } // نوع پنل (مثلاً "theme", "plugin")
    public string Priority { get; set; } // اولویت نمایش پنل
    public ICollection<PanelSetting> Settings { get; set; }
}

public class PanelSetting
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public int PanelId { get; set; }
    public Panel Panel { get; set; }
}