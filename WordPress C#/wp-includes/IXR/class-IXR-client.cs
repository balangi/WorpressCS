using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IXR
{
    /// <summary>
    /// Represents an XML-RPC client for making remote procedure calls.
    /// </summary>
    public class IXR_Client
    {
        // Properties
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string Path { get; private set; }
        public string UserAgent { get; private set; }
        public TimeSpan Timeout { get; private set; }
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        public bool Debug { get; set; }

        // Error handling
        public bool HasError => Error != null;
        public IXR_Error Error { get; private set; }

        // Response message
        private IXR_Message _message;

        /// <summary>
        /// Constructor to initialize the IXR_Client object.
        /// </summary>
        /// <param name="server">The server address (e.g., "example.com").</param>
        /// <param name="path">The path to the XML-RPC endpoint (optional).</param>
        /// <param name="port">The port number (default is 80).</param>
        /// <param name="timeout">The timeout in seconds (default is 15).</param>
        public IXR_Client(string server, string path = null, int port = 80, int timeout = 15)
        {
            if (string.IsNullOrEmpty(server))
                throw new ArgumentNullException(nameof(server), "Server cannot be null or empty.");

            var uri = new UriBuilder(server);
            Server = uri.Host;
            Port = uri.Port > 0 ? uri.Port : port;
            Path = !string.IsNullOrEmpty(path) ? path : uri.Path;
            Timeout = TimeSpan.FromSeconds(timeout);
            UserAgent = "The Incutio XML-RPC C# Library";
        }

        /// <summary>
        /// Sends a query to the XML-RPC server.
        /// </summary>
        /// <param name="method">The method name to call.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        /// <returns>True if the query was successful; otherwise, false.</returns>
        public async Task<bool> QueryAsync(string method, params object[] args)
        {
            try
            {
                // Create the XML request
                var request = new IXR_Request(method, args);
                var xml = request.GetXml();
                var contentLength = Encoding.UTF8.GetByteCount(xml);

                // Prepare the HTTP headers
                Headers["Host"] = Server;
                Headers["Content-Type"] = "text/xml";
                Headers["User-Agent"] = UserAgent;
                Headers["Content-Length"] = contentLength.ToString();

                // Build the HTTP request
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"http://{Server}:{Port}{Path}")
                {
                    Content = new StringContent(xml, Encoding.UTF8, "text/xml")
                };

                foreach (var header in Headers)
                {
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                // Send the HTTP request
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = Timeout;
                    var response = await httpClient.SendAsync(httpRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        Error = new IXR_Error(-32300, $"Transport error - HTTP status code was {response.StatusCode}");
                        return false;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (Debug)
                    {
                        Console.WriteLine("Response:");
                        Console.WriteLine(responseContent);
                    }

                    // Parse the response
                    _message = new IXR_Message(responseContent);
                    if (!_message.Parse())
                    {
                        Error = new IXR_Error(-32700, "Parse error - not well formed");
                        return false;
                    }

                    if (_message.MessageType == "fault")
                    {
                        Error = new IXR_Error(_message.FaultCode, _message.FaultString);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Error = new IXR_Error(-32300, $"Transport error - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the response from the XML-RPC server.
        /// </summary>
        /// <returns>The response parameter.</returns>
        public object GetResponse()
        {
            return _message?.Params[0];
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <returns>The error code.</returns>
        public int GetErrorCode()
        {
            return Error?.Code ?? 0;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <returns>The error message.</returns>
        public string GetErrorMessage()
        {
            return Error?.Message ?? string.Empty;
        }
    }

    /// <summary>
    /// Represents an XML-RPC request.
    /// </summary>
    public class IXR_Request
    {
        private readonly string _method;
        private readonly object[] _args;

        public IXR_Request(string method, object[] args)
        {
            _method = method;
            _args = args;
        }

        public string GetXml()
        {
            var xml = new StringBuilder();
            xml.Append($"<methodCall><methodName>{_method}</methodName><params>");

            foreach (var arg in _args)
            {
                xml.Append("<param>");
                xml.Append(SerializeValue(arg));
                xml.Append("</param>");
            }

            xml.Append("</params></methodCall>");
            return xml.ToString();
        }

        private string SerializeValue(object value)
        {
            switch (value)
            {
                case string str:
                    return $"<value><string>{str}</string></value>";
                case int i:
                    return $"<value><int>{i}</int></value>";
                case double d:
                    return $"<value><double>{d}</double></value>";
                case bool b:
                    return $"<value><boolean>{(b ? 1 : 0)}</boolean></value>";
                default:
                    throw new ArgumentException("Unsupported type for serialization.");
            }
        }

        public int GetLength()
        {
            return Encoding.UTF8.GetByteCount(GetXml());
        }
    }

    /// <summary>
    /// Represents an XML-RPC message.
    /// </summary>
    public class IXR_Message
    {
        private readonly string _xml;
        public string MessageType { get; private set; }
        public int FaultCode { get; private set; }
        public string FaultString { get; private set; }
        public List<object> Params { get; private set; } = new List<object>();

        public IXR_Message(string xml)
        {
            _xml = xml;
        }

        public bool Parse()
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(_xml);

                var methodResponseNode = xmlDoc.SelectSingleNode("methodResponse");
                if (methodResponseNode != null)
                {
                    var faultNode = methodResponseNode.SelectSingleNode("fault");
                    if (faultNode != null)
                    {
                        MessageType = "fault";
                        var faultValue = DeserializeValue(faultNode.SelectSingleNode("value"));
                        if (faultValue is Dictionary<string, object> faultDict)
                        {
                            FaultCode = (int)(double)faultDict["faultCode"];
                            FaultString = (string)faultDict["faultString"];
                        }
                        return true;
                    }

                    var paramsNode = methodResponseNode.SelectSingleNode("params");
                    if (paramsNode != null)
                    {
                        foreach (XmlNode paramNode in paramsNode.SelectNodes("param"))
                        {
                            Params.Add(DeserializeValue(paramNode.SelectSingleNode("value")));
                        }
                        MessageType = "response";
                        return true;
                    }
                }
            }
            catch
            {
                // Parsing error
            }

            return false;
        }

        private object DeserializeValue(XmlNode node)
        {
            if (node == null) return null;

            var child = node.FirstChild;
            switch (child.Name)
            {
                case "string":
                    return child.InnerText;
                case "int":
                case "i4":
                    return int.Parse(child.InnerText);
                case "double":
                    return double.Parse(child.InnerText);
                case "boolean":
                    return child.InnerText == "1";
                case "struct":
                    var dict = new Dictionary<string, object>();
                    foreach (XmlNode memberNode in child.SelectNodes("member"))
                    {
                        var name = memberNode.SelectSingleNode("name")?.InnerText;
                        var value = DeserializeValue(memberNode.SelectSingleNode("value"));
                        if (name != null) dict[name] = value;
                    }
                    return dict;
                default:
                    throw new ArgumentException("Unsupported XML-RPC type.");
            }
        }
    }

    /// <summary>
    /// Represents an XML-RPC error.
    /// </summary>
    public class IXR_Error
    {
        public int Code { get; }
        public string Message { get; }

        public IXR_Error(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}