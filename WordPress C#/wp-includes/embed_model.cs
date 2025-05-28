public class EmbedData
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string AuthorUrl { get; set; }
    public string ProviderName { get; set; }
    public string ProviderUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string Html { get; set; }
    public string Type { get; set; } // video, audio, rich, link
}