public class DeprecatedFunction
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Replacement { get; set; }
    public string VersionDeprecated { get; set; }
    public string VersionRemoved { get; set; }
    public bool IsRemoved => !string.IsNullOrEmpty(VersionRemoved);
}