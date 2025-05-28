public class FatalErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IErrorService _errorService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FatalErrorHandlerMiddleware(
        RequestDelegate next,
        IErrorService errorService,
        IHttpContextAccessor httpContextAccessor)
    {
        _next = next;
        _errorService = errorService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var file = Path.GetFileName(exception.StackTrace?.Split('\n')?.FirstOrDefault()?.Trim());
        var line = 0; // در C# خطوط خطای دقیق قابل دسترسی نیستند بدون Symbol Server

        await _errorService.LogErrorAsync(exception, file, line);

        var recoveryMode = new RecoveryModeService(_httpContextAccessor);
        if (recoveryMode.IsRecoveryModeEnabled())
        {
            recoveryMode.EnterRecoveryMode();
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "text/html";

        var description = await _errorService.GetHumanReadableDescription(exception, file, line);
        var html = $@"
            <div style='background:#fff;border:1px solid #eacda1;padding:10px;margin:10px auto;max-width:800px;font-family:sans-serif;color:#333;'>
                <h2>Fatal Error Detected</h2>
                <p>{description}</p>
                <pre>{exception.StackTrace}</pre>
            </div>";

        await context.Response.WriteAsync(html);
    }
}