public class SiteQueryParameters
{
    public List<int> SiteIds { get; set; }
    public string Domain { get; set; }
    public string Path { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsArchived { get; set; }
    public bool? IsSpam { get; set; }
    public string OrderBy { get; set; } = "id";
    public string Order { get; set; } = "asc";
    public int Limit { get; set; } = 100;
}