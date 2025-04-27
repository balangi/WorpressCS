public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public ICollection<MenuItem> Items { get; set; }
}

public class MenuItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public int Order { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; }
}