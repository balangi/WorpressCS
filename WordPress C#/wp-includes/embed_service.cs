public class EmbedService : IEmbedService
{
    private readonly Dictionary<string, (string Pattern, Func<string, Dictionary<string, object>, string> Handler)> _handlers = new();
    private readonly ApplicationDbContext _context;

    public EmbedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmbedData> GetEmbedDataAsync(string url, int? width = null, int? height = null)
    {
        // Simulate fetching embed data from external source
        if (url.Contains("youtube.com"))
        {
            var videoId = ExtractYouTubeVideoId(url);
            return new EmbedData
            {
                Url = url,
                Title = "Sample YouTube Video",
                AuthorName = "YouTube",
                AuthorUrl = "https://www.youtube.com ",
                ProviderName = "YouTube",
                ProviderUrl = "https://www.youtube.com ",
                ThumbnailUrl = $"https://i.ytimg.com/vi/ {videoId}/hqdefault.jpg",
                Width = width ?? 600,
                Height = height ?? 400,
                Html = $"<iframe width='{width}' height='{height}' src='https://www.youtube.com/embed/ {videoId}' frameborder='0' allowfullscreen></iframe>",
                Type = "video"
            };
        }

        return null;
    }

    private string ExtractYouTubeVideoId(string url)
    {
        var match = Regex.Match(url, @"(?:v=|\/)([a-zA-Z0-9\-_]+)(?:\?|&|$)");
        return match.Success ? match.Groups[1].Value : "";
    }

    public async Task<string> RenderEmbedHtmlAsync(EmbedData data)
    {
        if (data == null) return "";

        var html = new StringBuilder();

        html.AppendLine($"<blockquote class=\"wp-embedded-content\" data-url=\"{data.Url}\">");
        html.AppendLine($"<a href=\"{data.Url}\">{data.Title}</a>");
        html.AppendLine("</blockquote>");

        if (!string.IsNullOrEmpty(data.Html))
        {
            html.AppendLine(data.Html);
        }

        return html.ToString();
    }

    public async Task RegisterDefaultHandlersAsync()
    {
        await AddCustomHandlerAsync("youtube", @"https?://(www.)?youtube\.com/(?:v|embed)/([^/]+)", HandleYouTubeEmbed);
        await AddCustomHandlerAsync("audio", @"^https?://.+?\.(mp3|ogg|wav)$", HandleAudioEmbed);
        await AddCustomHandlerAsync("video", @"^https?://.+?\.(mp4|webm|ogv)$", HandleVideoEmbed);
    }

    private string HandleYouTubeEmbed(string url, Dictionary<string, object> args)
    {
        var width = args.ContainsKey("width") ? Convert.ToInt32(args["width"]) : 600;
        var height = args.ContainsKey("height") ? Convert.ToInt32(args["height"]) : 400;
        var videoId = ExtractYouTubeVideoId(url);

        return $"<iframe width='{width}' height='{height}' src='https://www.youtube.com/embed/ {videoId}' frameborder='0' allowfullscreen></iframe>";
    }

    private string HandleAudioEmbed(string url, Dictionary<string, object> args)
    {
        return $"<audio controls><source src='{url}' type='audio/mpeg'>Your browser does not support the audio element.</audio>";
    }

    private string HandleVideoEmbed(string url, Dictionary<string, object> args)
    {
        return $"<video width='100%' controls><source src='{url}' type='video/mp4'>Your browser does not support the video tag.</video>";
    }

    public async Task<bool> AddCustomHandlerAsync(string id, string pattern, Func<string, Dictionary<string, object>, string> handler, int priority = 10)
    {
        if (_handlers.ContainsKey(id)) return false;

        _handlers[id] = (pattern, handler);
        return true;
    }
}