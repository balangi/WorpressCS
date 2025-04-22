using System;
using System.Collections.Generic;

namespace IXR
{
    /// <summary>
    /// Represents an introspection server for XML-RPC, providing metadata about available methods.
    /// </summary>
    public class IXR_IntrospectionServer : IXR_Server
    {
        // Dictionaries to store method signatures, help messages, and capabilities
        private readonly Dictionary<string, Delegate> _callbacks = new Dictionary<string, Delegate>();
        private readonly Dictionary<string, List<string>> _signatures = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, string> _help = new Dictionary<string, string>();

        /// <summary>
        /// Constructor to initialize the IXR_IntrospectionServer object.
        /// </summary>
        public IXR_IntrospectionServer()
        {
            SetCallbacks();
            SetCapabilities();

            // Add introspection capabilities
            Capabilities["introspection"] = new Dictionary<string, object>
            {
                { "specUrl", "http://xmlrpc.usefulinc.com/doc/reserved.html" },
                { "specVersion", 1 }
            };

            // Register introspection methods
            AddCallback("system.methodSignature", MethodSignature, new List<string> { "array", "string" }, "Returns an array describing the return type and required parameters of a method");
            AddCallback("system.getCapabilities", GetCapabilities, new List<string> { "struct" }, "Returns a struct describing the XML-RPC specifications supported by this server");
            AddCallback("system.listMethods", ListMethods, new List<string> { "array" }, "Returns an array of available methods on this server");
            AddCallback("system.methodHelp", MethodHelp, new List<string> { "string", "string" }, "Returns a documentation string for the specified method");
        }

        /// <summary>
        /// Adds a callback method with its signature and help message.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <param name="callback">The callback function.</param>
        /// <param name="args">The method signature (list of types).</param>
        /// <param name="help">The help message for the method.</param>
        public void AddCallback(string method, Delegate callback, List<string> args, string help)
        {
            _callbacks[method] = callback;
            _signatures[method] = args;
            _help[method] = help;
        }

        /// <summary>
        /// Calls a method by name with the given arguments.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        /// <returns>The result of the method call or an error.</returns>
        public override object Call(string methodName, params object[] args)
        {
            // Ensure arguments are in an array
            if (args == null)
            {
                args = Array.Empty<object>();
            }

            // Check if the method exists
            if (!_callbacks.ContainsKey(methodName))
            {
                return new IXR_Error(-32601, $"server error. requested method \"{methodName}\" not specified.");
            }

            var signature = _signatures[methodName];
            var returnType = signature[0];
            signature.RemoveAt(0);

            // Check the number of arguments
            if (args.Length != signature.Count)
            {
                return new IXR_Error(-32602, "server error. wrong number of method parameters");
            }

            // Validate argument types
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var expectedType = signature[i];

                switch (expectedType)
                {
                    case "int":
                    case "i4":
                        if (!(arg is int))
                        {
                            return new IXR_Error(-32602, "server error. invalid method parameters");
                        }
                        break;

                    case "string":
                        if (!(arg is string))
                        {
                            return new IXR_Error(-32602, "server error. invalid method parameters");
                        }
                        break;

                    case "boolean":
                        if (!(arg is bool))
                        {
                            return new IXR_Error(-32602, "server error. invalid method parameters");
                        }
                        break;

                    case "double":
                        if (!(arg is double))
                        {
                            return new IXR_Error(-32602, "server error. invalid method parameters");
                        }
                        break;

                    case "dateTime.iso8601":
                        if (!(arg is IXR_Date))
                        {
                            return new IXR_Error(-32602, "server error. invalid method parameters");
                        }
                        break;

                    case "base64":
                        if (!(arg is IXR_Base64))
                        {
                            return new IXR_Error(-32602, "server error. invalid method parameters");
                        }
                        break;

                    default:
                        return new IXR_Error(-32602, "server error. unsupported parameter type");
                }
            }

            // Call the actual method
            return _callbacks[methodName].DynamicInvoke(args);
        }

        /// <summary>
        /// Returns the signature of a method.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <returns>An array of example values representing the method signature.</returns>
        public object MethodSignature(string method)
        {
            if (!_signatures.ContainsKey(method))
            {
                return new IXR_Error(-32601, $"server error. requested method \"{method}\" not specified.");
            }

            var types = _signatures[method];
            var result = new List<object>();

            foreach (var type in types)
            {
                switch (type)
                {
                    case "string":
                        result.Add("string");
                        break;

                    case "int":
                    case "i4":
                        result.Add(42);
                        break;

                    case "double":
                        result.Add(3.1415);
                        break;

                    case "dateTime.iso8601":
                        result.Add(new IXR_Date(DateTime.UtcNow));
                        break;

                    case "boolean":
                        result.Add(true);
                        break;

                    case "base64":
                        result.Add(new IXR_Base64(new byte[] { 65, 66, 67 }));
                        break;

                    case "array":
                        result.Add(new List<object> { "array" });
                        break;

                    case "struct":
                        result.Add(new Dictionary<string, object> { { "struct", "struct" } });
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported type: {type}");
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the help message for a method.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <returns>The help message for the method.</returns>
        public string MethodHelp(string method)
        {
            return _help.ContainsKey(method) ? _help[method] : string.Empty;
        }

        /// <summary>
        /// Lists all available methods.
        /// </summary>
        /// <returns>A list of method names.</returns>
        public List<string> ListMethods()
        {
            return new List<string>(_callbacks.Keys);
        }

        /// <summary>
        /// Gets the capabilities of the server.
        /// </summary>
        /// <returns>A dictionary of capabilities.</returns>
        public Dictionary<string, object> GetCapabilities()
        {
            return Capabilities;
        }
    }
}