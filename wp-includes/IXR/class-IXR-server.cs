using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace IXR
{
    /// <summary>
    /// Represents an XML-RPC server.
    /// </summary>
    public class IXR_Server
    {
        // Properties
        private readonly Dictionary<string, Delegate> _callbacks = new Dictionary<string, Delegate>();
        private readonly Dictionary<string, object> _capabilities = new Dictionary<string, object>();

        /// <summary>
        /// Constructor to initialize the IXR_Server object.
        /// </summary>
        /// <param name="callbacks">Optional dictionary of callbacks.</param>
        /// <param name="data">Optional raw XML data.</param>
        /// <param name="wait">If true, the server will not serve immediately.</param>
        public IXR_Server(Dictionary<string, Delegate> callbacks = null, string data = null, bool wait = false)
        {
            SetCapabilities();
            if (callbacks != null)
            {
                foreach (var callback in callbacks)
                {
                    _callbacks[callback.Key] = callback.Value;
                }
            }
            SetCallbacks();

            if (!wait)
            {
                Serve(data);
            }
        }

        /// <summary>
        /// Serves the XML-RPC request.
        /// </summary>
        /// <param name="data">Optional raw XML data.</param>
        public void Serve(string data = null)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    if (HttpContext.Current.Request.HttpMethod != "POST")
                    {
                        HttpContext.Current.Response.StatusCode = 405;
                        HttpContext.Current.Response.AppendHeader("Allow", "POST");
                        HttpContext.Current.Response.ContentType = "text/plain";
                        HttpContext.Current.Response.Write("XML-RPC server accepts POST requests only.");
                        HttpContext.Current.Response.End();
                        return;
                    }

                    using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
                    {
                        data = reader.ReadToEnd();
                    }
                }

                var message = new IXR_Message(data);
                if (!message.Parse())
                {
                    Error(new IXR_Error(-32700, "Parse error. Not well formed"));
                    return;
                }

                if (message.MessageType != "methodCall")
                {
                    Error(new IXR_Error(-32600, "Server error. Invalid XML-RPC. Request must be a methodCall"));
                    return;
                }

                var result = Call(message.MethodName, message.Params);

                if (result is IXR_Error error)
                {
                    Error(error);
                    return;
                }

                var r = new IXR_Value(result);
                var resultXml = r.GetXml();

                var xml = $@"
<methodResponse>
  <params>
    <param>
      <value>
      {resultXml}
      </value>
    </param>
  </params>
</methodResponse>";

                Output(xml);
            }
            catch (Exception ex)
            {
                Error(new IXR_Error(-32603, $"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Calls a method by name with the given arguments.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        /// <returns>The result of the method call or an error.</returns>
        public object Call(string methodName, List<object> args)
        {
            if (!_callbacks.ContainsKey(methodName))
            {
                return new IXR_Error(-32601, $"Server error. Requested method '{methodName}' does not exist.");
            }

            var method = _callbacks[methodName];

            // If there's only one argument, pass it directly instead of the whole array
            if (args.Count == 1)
            {
                args = new List<object> { args[0] };
            }

            try
            {
                return method.DynamicInvoke(args.ToArray());
            }
            catch (Exception ex)
            {
                return new IXR_Error(-32602, $"Server error. Failed to invoke method '{methodName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Outputs an error response.
        /// </summary>
        /// <param name="error">The error object.</param>
        private void Error(IXR_Error error)
        {
            var xml = error.GetXml();
            Output(xml);
        }

        /// <summary>
        /// Outputs the XML response.
        /// </summary>
        /// <param name="xml">The XML string to output.</param>
        private void Output(string xml)
        {
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(xml);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// Checks if a method exists.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <returns>True if the method exists; otherwise, false.</returns>
        private bool HasMethod(string method)
        {
            return _callbacks.ContainsKey(method);
        }

        /// <summary>
        /// Sets the capabilities of the server.
        /// </summary>
        private void SetCapabilities()
        {
            _capabilities["xmlrpc"] = new Dictionary<string, object>
            {
                { "specUrl", "http://www.xmlrpc.com/spec" },
                { "specVersion", 1 }
            };
            _capabilities["faults_interop"] = new Dictionary<string, object>
            {
                { "specUrl", "http://xmlrpc-epi.sourceforge.net/specs/rfc.fault_codes.php" },
                { "specVersion", 20010516 }
            };
            _capabilities["system.multicall"] = new Dictionary<string, object>
            {
                { "specUrl", "http://www.xmlrpc.com/discuss/msgReader$1208" },
                { "specVersion", 1 }
            };
        }

        /// <summary>
        /// Sets the default callbacks for the server.
        /// </summary>
        private void SetCallbacks()
        {
            _callbacks["system.getCapabilities"] = new Func<object[], object>(GetCapabilities);
            _callbacks["system.listMethods"] = new Func<object[], object>(ListMethods);
            _callbacks["system.multicall"] = new Func<object[], object>(MultiCall);
        }

        /// <summary>
        /// Returns the capabilities of the server.
        /// </summary>
        /// <param name="args">Optional arguments (not used).</param>
        /// <returns>A dictionary of capabilities.</returns>
        public object GetCapabilities(object[] args)
        {
            return _capabilities;
        }

        /// <summary>
        /// Lists all available methods.
        /// </summary>
        /// <param name="args">Optional arguments (not used).</param>
        /// <returns>A list of method names.</returns>
        public object ListMethods(object[] args)
        {
            return _callbacks.Keys.Reverse().ToList();
        }

        /// <summary>
        /// Handles multicall requests.
        /// </summary>
        /// <param name="args">An array of method calls.</param>
        /// <returns>A list of results for each method call.</returns>
        public object MultiCall(object[] args)
        {
            var methodCalls = args[0] as List<Dictionary<string, object>>;
            var results = new List<object>();

            foreach (var call in methodCalls)
            {
                var method = call["methodName"].ToString();
                var parameters = call["params"] as List<object>;

                if (method == "system.multicall")
                {
                    results.Add(new Dictionary<string, object>
                    {
                        { "faultCode", -32600 },
                        { "faultString", "Recursive calls to system.multicall are forbidden" }
                    });
                    continue;
                }

                var result = Call(method, parameters);

                if (result is IXR_Error error)
                {
                    results.Add(new Dictionary<string, object>
                    {
                        { "faultCode", error.Code },
                        { "faultString", error.Message }
                    });
                }
                else
                {
                    results.Add(new List<object> { result });
                }
            }

            return results;
        }
    }
}