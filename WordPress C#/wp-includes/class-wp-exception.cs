using System;

namespace WordPress.Core.Exceptions
{
    /// <summary>
    /// Base Exception class for WordPress Core in C#.
    /// Future, more specific Exceptions should always extend this base class.
    /// </summary>
    public class WP_Exception : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WP_Exception"/> class.
        /// </summary>
        public WP_Exception()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WP_Exception"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public WP_Exception(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WP_Exception"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public WP_Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}