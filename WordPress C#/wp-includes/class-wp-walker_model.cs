public class TreeElement
{
    public int Id { get; set; }
    public int? ParentId { get; set; } // null means root
    public string Name { get; set; }
}