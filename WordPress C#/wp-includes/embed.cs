[ApiController]
[Route("oembed/1.0/[action]")]
public class EmbedController : ControllerBase
{
    private readonly IEmbedService _embedService;

    public EmbedController(IEmbedService embedService) => _embedService = embedService;

    [HttpGet]
    public async Task<IActionResult> Embed([FromQuery] string url, [FromQuery] string format = "json", [FromQuery] int? maxwidth = null, [FromQuery] int? maxheight = null)
    {
        if (string.IsNullOrEmpty(url))
            return BadRequest("Missing URL");

        var data = await _embedService.GetEmbedDataAsync(url, maxwidth, maxheight);

        if (data == null)
            return NotFound("No embed found for this URL.");

        if (format == "xml")
        {
            var xml = GenerateXmlResponse(data);
            return Content(xml, "text/xml");
        }

        return Ok(data);
    }

    private string GenerateXmlResponse(EmbedData data)
    {
        var doc = new XDocument(
            new XElement("oembed",
                new XElement("title", data.Title),
                new XElement("author_name", data.AuthorName),
                new XElement("author_url", data.AuthorUrl),
                new XElement("provider_name", data.ProviderName),
                new XElement("provider_url", data.ProviderUrl),
                new XElement("thumbnail_url", data.ThumbnailUrl),
                new XElement("width", data.Width),
                new XElement("height", data.Height),
                new XElement("html", data.Html)
            )
        );

        return doc.ToString();
    }
}