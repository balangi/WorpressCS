using System;
using System.Collections.Generic;
using System.Text;

namespace IXR
{
    /// <summary>
    /// Represents an XML-RPC request.
    /// </summary>
    public class IXR_Request
    {
        // Properties
        public string Method { get; private set; }
        public List<object> Args { get; private set; }
        public string Xml { get; private set; }

        /// <summary>
        /// Constructor to initialize the IXR_Request object.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        public IXR_Request(string method, List<object> args)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method), "Method cannot be null.");
            Args = args ?? new List<object>();

            // Build the XML request
            var xmlBuilder = new StringBuilder();
            xmlBuilder.AppendLine(@"<?xml version=""1.0""?>");
            xmlBuilder.AppendLine("<methodCall>");
            xmlBuilder.AppendLine($"<methodName>{Method}</methodName>");
            xmlBuilder.AppendLine("<params>");

            foreach (var arg in Args)
            {
                xmlBuilder.AppendLine("<param><value>");
                var value = new IXR_Value(arg);
                xmlBuilder.AppendLine(value.GetXml());
                xmlBuilder.AppendLine("</value></param>");
            }

            xmlBuilder.AppendLine("</params>");
            xmlBuilder.AppendLine("</methodCall>");

            Xml = xmlBuilder.ToString();
        }

        /// <summary>
        /// Gets the length of the XML request.
        /// </summary>
        /// <returns>The length of the XML string.</returns>
        public int GetLength()
        {
            return Xml.Length;
        }

        /// <summary>
        /// Gets the XML representation of the request.
        /// </summary>
        /// <returns>The XML string.</returns>
        public string GetXml()
        {
            return Xml;
        }
    }

    /// <summary>
    /// Represents a value in XML-RPC format.
    /// </summary>
    public class IXR_Value
    {
        private readonly object _value;

        /// <summary>
        /// Constructor to initialize the IXR_Value object.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        public IXR_Value(object value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the XML representation of the value.
        /// </summary>
        /// <returns>The XML string for the value.</returns>
        public string GetXml()
        {
            if (_value == null)
                return "<nil/>";

            switch (_value)
            {
                case string str:
                    return $"<string>{str}</string>";
                case int i:
                    return $"<int>{i}</int>";
                case double d:
                    return $"<double>{d}</double>";
                case bool b:
                    return $"<boolean>{(b ? 1 : 0)}</boolean>";
                case DateTime dt:
                    var isoDate = dt.ToString("yyyyMMddTHHmmssZ");
                    return $"<dateTime.iso8601>{isoDate}</dateTime.iso8601>";
                case byte[] bytes:
                    var base64 = Convert.ToBase64String(bytes);
                    return $"<base64>{base64}</base64>";
                default:
                    throw new ArgumentException($"Unsupported type: {_value.GetType().Name}");
            }
        }
    }
}