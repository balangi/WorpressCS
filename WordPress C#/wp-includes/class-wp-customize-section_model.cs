public class Section
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; } // نوع بخش (مثلاً "theme", "plugin")
    public string Priority { get; set; } // اولویت نمایش بخش
    public ICollection<SectionSetting> Settings { get; set; }
}

public class SectionSetting
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public int SectionId { get; set; }
    public Section Section { get; set; }
}