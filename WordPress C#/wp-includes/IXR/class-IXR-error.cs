using System;
using System.Net;

namespace IXR
{
    /// <summary>
    /// Represents an error in XML-RPC with a fault code and message.
    /// </summary>
    public class IXR_Error : Exception
    {
        // Properties for the error code and message
        public int Code { get; private set; }
        public string Message { get; private set; }

        /// <summary>
        /// Constructor to initialize the IXR_Error object.
        /// </summary>
        /// <param name="code">The fault code.</param>
        /// <param name="message">The fault message.</param>
        public IXR_Error(int code, string message)
            : base(message) // Base constructor of Exception
        {
            Code = code;
            Message = WebUtility.HtmlEncode(message); // Encode the message for XML safety
        }

        /// <summary>
        /// Gets the XML representation of the error.
        /// </summary>
        /// <returns>An XML string representing the error.</returns>
        public string GetXml()
        {
            return $@"
<methodResponse>
  <fault>
    <value>
      <struct>
        <member>
          <name>faultCode</name>
          <value><int>{Code}</int></value>
        </member>
        <member>
          <name>faultString</name>
          <value><string>{Message}</string></value>
        </member>
      </struct>
    </value>
  </fault>
</methodResponse>";
        }
    }
}