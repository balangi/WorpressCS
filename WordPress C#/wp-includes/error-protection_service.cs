public class ErrorService : IErrorService
{
    private readonly ApplicationDbContext _context;

    public ErrorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogErrorAsync(Exception exception, string file, int line)
    {
        var error = new ErrorLog
        {
            ErrorMessage = exception.Message,
            ErrorType = exception.GetType().Name,
            File = file,
            Line = line,
            StackTrace = exception.StackTrace
        };

        await _context.ErrorLogs.AddAsync(error);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ErrorLog>> GetRecentErrors(int count = 10)
    {
        return await _context.ErrorLogs
            .OrderByDescending(e => e.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<string> GetHumanReadableDescription(Exception ex, string file, int line)
    {
        return $@"An error of type <code>{ex.GetType().Name}</code> was caused in line 
                <code>{line}</code> of the file <code>{file}</code>. 
                Error message: <code>{ex.Message}</code>";
    }

    public async Task ClearAllErrors()
    {
        _context.ErrorLogs.RemoveRange(_context.ErrorLogs);
        await _context.SaveChangesAsync();
    }
}