public class ErrorLog
{
    public int Id { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorType { get; set; } // E_ERROR, E_WARNING, etc.
    public string File { get; set; }
    public int Line { get; set; }
    public string StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}