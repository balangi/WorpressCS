public class DatabaseConnectionException : WP_Exception
{
    public DatabaseConnectionException(string message)
        : base(message)
    {
    }

    public DatabaseConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}