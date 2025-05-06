public class NavigationMenu
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    public DateTime PublishDate { get; set; }
}

public class ClassicMenu
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}