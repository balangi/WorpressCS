// Bookmark.cs
public class Bookmark
{
    public int Id { get; set; }
    public string LinkUrl { get; set; }
    public string LinkName { get; set; }
    public string LinkDescription { get; set; }
    public string LinkImage { get; set; }
    public string LinkTarget { get; set; }
    public string LinkRel { get; set; }
    public DateTime LinkUpdated { get; set; }
    public bool RecentlyUpdated { get; set; }
    public int LinkRating { get; set; }
    public bool IsVisible { get; set; }
    public int CategoryId { get; set; }
    public BookmarkCategory Category { get; set; }
}

// BookmarkCategory.cs
public class BookmarkCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Bookmark> Bookmarks { get; set; }
}