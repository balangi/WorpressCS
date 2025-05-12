[ApiController]
[Route("api/[controller]")]
public class WidgetController : ControllerBase
{
    private readonly IWidgetFactory _widgetFactory;

    public WidgetController(IWidgetFactory widgetFactory) => _widgetFactory = widgetFactory;

    [HttpGet("{idBase}")]
    public IActionResult RenderWidget(string idBase)
    {
        var widget = _widgetFactory.GetWidget(idBase);
        if (widget == null)
        {
            return NotFound();
        }

        var args = new Dictionary<string, object>
        {
            {"before_title", "<h3>"}, {"after_title", "</h3>"}
        };
        var instance = new Dictionary<string, object>
        {
            {"title", "Latest News"}
        };

        var output = new StringWriter();
        using (var writer = new HtmlTextWriter(output))
        {
            writer.Write($"<div class=\"widget {idBase}\">");
            widget.Widget(args, instance);
            writer.Write("</div>");
        }

        return Ok(output.ToString());
    }

    [HttpPost("{idBase}/update")]
    public IActionResult UpdateSettings(string idBase, [FromBody] Dictionary<string, object> data)
    {
        var widget = _widgetFactory.GetWidget(idBase);
        if (widget == null)
        {
            return NotFound();
        }

        var oldSettings = new Dictionary<string, object> { { "title", "Old Title" } };
        var newSettings = widget.Update(data, oldSettings);

        return Ok(newSettings);
    }
}