public interface IErrorService
{
    Task LogErrorAsync(Exception exception, string file, int line);
    Task<List<ErrorLog>> GetRecentErrors(int count = 10);
    Task<string> GetHumanReadableDescription(Exception ex, string file, int line);
    Task ClearAllErrors();
}