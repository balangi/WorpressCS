[ApiController]
[Route("api/[controller]")]
public class WidgetsController : ControllerBase
{
    private readonly IWidgetService _widgetService;

    public WidgetsController(IWidgetService widgetService) => _widgetService = widgetService;

    [HttpGet("{widgetId}")]
    public async Task<IActionResult> GetWidget(string widgetId)
    {
        var settings = await _widgetService.GetSettingsAsync(widgetId);
        return Ok(settings);
    }

    [HttpPost("{widgetId}")]
    public async Task<IActionResult> UpdateWidget(string widgetId, [FromBody] Dictionary<string, object> data)
    {
        await _widgetService.SaveSettingsAsync(widgetId, data);
        return Ok(new { success = true });
    }
}