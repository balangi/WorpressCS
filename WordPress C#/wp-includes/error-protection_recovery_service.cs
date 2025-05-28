public class RecoveryModeService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RecoveryModeService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsRecoveryModeEnabled()
    {
        // در ASP.NET Core، می‌توانید این را در appsettings.json یا Environment Variables قرار دهید
        return true;
    }

    public void EnterRecoveryMode()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        context.Response.Cookies.Append("wp-recovery-mode", "1");
    }

    public void ExitRecoveryMode()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        context.Response.Cookies.Delete("wp-recovery-mode");
    }

    public bool InRecoveryMode()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.Request.Cookies.ContainsKey("wp-recovery-mode") ?? false;
    }
}