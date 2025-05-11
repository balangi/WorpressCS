public class TermQueryParameters
{
    public string Taxonomy { get; set; }
    public List<int> Include { get; set; }
    public List<int> Exclude { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }
    public string OrderBy { get; set; } = "id";
    public string Order { get; set; } = "asc";
    public int Limit { get; set; } = 100;
}