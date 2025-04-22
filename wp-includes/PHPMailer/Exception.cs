using System;
using System.Web;

namespace PHPMailer.PHPMailer
{
    /// <summary>
    /// Represents a custom exception handler for PHPMailer in C#.
    /// </summary>
    public class Exception : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public Exception(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public Exception(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Formats the error message for HTML output.
        /// </summary>
        /// <returns>A formatted string containing the error message.</returns>
        public string ErrorMessage()
        {
            return $"<strong>{HttpUtility.HtmlEncode(Message)}</strong><br />\n";
        }
    }
}