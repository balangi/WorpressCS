using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockBindings
{
    public class BlockBindingsSource
    {
        /// <summary>
        /// The name of the source.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The label of the source.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The function used to get the value from the source.
        /// </summary>
        private Func<Dictionary<string, object>, object, string, object> _getValueCallback;

        /// <summary>
        /// The context added to the blocks needed by the source.
        /// </summary>
        public List<string> UsesContext { get; set; } = new List<string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <param name="sourceProperties">The properties of the source.</param>
        public BlockBindingsSource(string name, Dictionary<string, object> sourceProperties)
        {
            Name = name;

            if (sourceProperties.ContainsKey("label"))
            {
                Label = sourceProperties["label"].ToString();
            }

            if (sourceProperties.ContainsKey("get_value_callback") && sourceProperties["get_value_callback"] is Delegate callback)
            {
                _getValueCallback = (Func<Dictionary<string, object>, object, string, object>)callback;
            }

            if (sourceProperties.ContainsKey("uses_context") && sourceProperties["uses_context"] is List<string> usesContext)
            {
                UsesContext = usesContext;
            }
        }

        /// <summary>
        /// Calls the callback function specified in the `_getValueCallback` property
        /// with the given arguments and returns the result. It can be modified with
        /// `block_bindings_source_value` filter.
        /// </summary>
        public object GetValue(Dictionary<string, object> sourceArgs, object blockInstance, string attributeName)
        {
            var value = _getValueCallback?.Invoke(sourceArgs, blockInstance, attributeName);

            // Simulate the `block_bindings_source_value` filter
            value = ApplyFilters(value, Name, sourceArgs, blockInstance, attributeName);

            return value;
        }

        /// <summary>
        /// Simulates the `block_bindings_source_value` filter.
        /// </summary>
        private object ApplyFilters(object value, string name, Dictionary<string, object> sourceArgs, object blockInstance, string attributeName)
        {
            // Example: A custom filter logic can be implemented here
            Console.WriteLine($"Filter applied for source '{name}' with attribute '{attributeName}'");

            return value;
        }

        /// <summary>
        /// Wakeup magic method.
        /// </summary>
        public void __Wakeup()
        {
            throw new InvalidOperationException($"{nameof(BlockBindingsSource)} should never be unserialized.");
        }
    }
}