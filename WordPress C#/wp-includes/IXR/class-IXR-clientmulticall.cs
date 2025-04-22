using System;
using System.Collections.Generic;

namespace IXR
{
    /// <summary>
    /// Represents a multicall client for making multiple XML-RPC calls in a single request.
    /// </summary>
    public class IXR_ClientMulticall : IXR_Client
    {
        // List to store multiple calls
        private readonly List<Dictionary<string, object>> _calls = new List<Dictionary<string, object>>();

        /// <summary>
        /// Constructor to initialize the IXR_ClientMulticall object.
        /// </summary>
        /// <param name="server">The server address (e.g., "example.com").</param>
        /// <param name="path">The path to the XML-RPC endpoint (optional).</param>
        /// <param name="port">The port number (default is 80).</param>
        public IXR_ClientMulticall(string server, string path = null, int port = 80)
            : base(server, path, port)
        {
            UserAgent = "The Incutio XML-RPC C# Library (multicall client)";
        }

        /// <summary>
        /// Adds a method call to the multicall list.
        /// </summary>
        /// <param name="methodName">The method name to call.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        public void AddCall(string methodName, params object[] args)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(nameof(methodName), "Method name cannot be null or empty.");

            var call = new Dictionary<string, object>
            {
                { "methodName", methodName },
                { "params", args }
            };

            _calls.Add(call);
        }

        /// <summary>
        /// Sends all added calls as a single multicall request to the XML-RPC server.
        /// </summary>
        /// <returns>True if the query was successful; otherwise, false.</returns>
        public async Task<bool> QueryAsync()
        {
            try
            {
                // Prepare the multicall request
                var multicallParams = new List<Dictionary<string, object>>();
                foreach (var call in _calls)
                {
                    var multicallParam = new Dictionary<string, object>
                    {
                        { "methodName", call["methodName"] },
                        { "params", call["params"] }
                    };
                    multicallParams.Add(multicallParam);
                }

                // Call the parent QueryAsync method with "system.multicall"
                return await base.QueryAsync("system.multicall", multicallParams);
            }
            catch (Exception ex)
            {
                Error = new IXR_Error(-32300, $"Transport error - {ex.Message}");
                return false;
            }
        }
    }
}