using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockBindings
{
    public class BlockBindingsRegistry
    {
        /// <summary>
        /// Holds the registered block bindings sources, keyed by source identifier.
        /// </summary>
        private Dictionary<string, BlockBindingSource> _sources = new Dictionary<string, BlockBindingSource>();

        /// <summary>
        /// Container for the main instance of the class.
        /// </summary>
        private static BlockBindingsRegistry _instance;

        /// <summary>
        /// Supported source properties that can be passed to the registered source.
        /// </summary>
        private readonly List<string> _allowedSourceProperties = new List<string>
        {
            "label",
            "get_value_callback",
            "uses_context"
        };

        /// <summary>
        /// Supported blocks that can use the block bindings API.
        /// </summary>
        private readonly List<string> _supportedBlocks = new List<string>
        {
            "core/paragraph",
            "core/heading",
            "core/image",
            "core/button"
        };

        private readonly AppDbContext _dbContext;

        public BlockBindingsRegistry(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Registers a new block bindings source.
        /// </summary>
        public BlockBindingSource Register(string sourceName, Dictionary<string, object> sourceProperties)
        {
            if (string.IsNullOrEmpty(sourceName))
            {
                throw new ArgumentException("Block bindings source name must be a string.");
            }

            if (Regex.IsMatch(sourceName, "[A-Z]+"))
            {
                throw new ArgumentException("Block bindings source names must not contain uppercase characters.");
            }

            if (!Regex.IsMatch(sourceName, @"^[a-z0-9-]+\/[a-z0-9-]+$"))
            {
                throw new ArgumentException("Block bindings source names must contain a namespace prefix. Example: my-plugin/my-custom-source");
            }

            if (_sources.ContainsKey(sourceName))
            {
                throw new InvalidOperationException($"Block bindings source \"{sourceName}\" already registered.");
            }

            if (!sourceProperties.ContainsKey("label"))
            {
                throw new ArgumentException("The $source_properties must contain a \"label\".");
            }

            if (!sourceProperties.ContainsKey("get_value_callback") || !(sourceProperties["get_value_callback"] is Delegate))
            {
                throw new ArgumentException("The \"get_value_callback\" parameter must be a valid callback.");
            }

            if (sourceProperties.ContainsKey("uses_context") && !(sourceProperties["uses_context"] is List<string>))
            {
                throw new ArgumentException("The \"uses_context\" parameter must be an array.");
            }

            var invalidProperties = sourceProperties.Keys.Except(_allowedSourceProperties);
            if (invalidProperties.Any())
            {
                throw new ArgumentException("The $source_properties array contains invalid properties.");
            }

            var source = new BlockBindingSource
            {
                SourceName = sourceName,
                Label = sourceProperties["label"].ToString(),
                GetValueCallback = sourceProperties["get_value_callback"].ToString(),
                UsesContext = sourceProperties.ContainsKey("uses_context")
                    ? (List<string>)sourceProperties["uses_context"]
                    : new List<string>()
            };

            _sources[sourceName] = source;

            return source;
        }

        /// <summary>
        /// Unregisters a block bindings source.
        /// </summary>
        public BlockBindingSource Unregister(string sourceName)
        {
            if (!_sources.ContainsKey(sourceName))
            {
                throw new KeyNotFoundException($"Block binding \"{sourceName}\" not found.");
            }

            var unregisteredSource = _sources[sourceName];
            _sources.Remove(sourceName);

            return unregisteredSource;
        }

        /// <summary>
        /// Retrieves the list of all registered block bindings sources.
        /// </summary>
        public Dictionary<string, BlockBindingSource> GetAllRegistered()
        {
            return _sources;
        }

        /// <summary>
        /// Retrieves a registered block bindings source.
        /// </summary>
        public BlockBindingSource GetRegistered(string sourceName)
        {
            return _sources.ContainsKey(sourceName) ? _sources[sourceName] : null;
        }

        /// <summary>
        /// Checks if a block bindings source is registered.
        /// </summary>
        public bool IsRegistered(string sourceName)
        {
            return _sources.ContainsKey(sourceName);
        }

        /// <summary>
        /// Utility method to retrieve the main instance of the class.
        /// </summary>
        public static BlockBindingsRegistry GetInstance(AppDbContext dbContext)
        {
            if (_instance == null)
            {
                _instance = new BlockBindingsRegistry(dbContext);
            }

            return _instance;
        }
    }
}