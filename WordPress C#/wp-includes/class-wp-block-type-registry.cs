using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockTypes
{
    public class BlockTypeRegistry
    {
        /// <summary>
        /// Registered block types dictionary.
        /// </summary>
        private readonly Dictionary<string, BlockType> _registeredBlockTypes = new Dictionary<string, BlockType>();

        /// <summary>
        /// Container for the main instance of the class.
        /// </summary>
        private static BlockTypeRegistry _instance;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private BlockTypeRegistry() { }

        /// <summary>
        /// Registers a block type.
        /// </summary>
        public BlockType Register(string name, Dictionary<string, object> args, AppDbContext dbContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Block type names must be strings.");
            }

            if (name.Any(char.IsUpper))
            {
                throw new ArgumentException("Block type names must not contain uppercase characters.");
            }

            if (!name.Contains("/"))
            {
                throw new ArgumentException("Block type names must contain a namespace prefix. Example: my-plugin/my-custom-block-type");
            }

            if (_registeredBlockTypes.ContainsKey(name))
            {
                throw new InvalidOperationException($"Block type \"{name}\" is already registered.");
            }

            var blockType = new BlockType
            {
                Name = name,
                Title = args.ContainsKey("title") ? args["title"].ToString() : string.Empty,
                Category = args.ContainsKey("category") ? args["category"].ToString() : null,
                Description = args.ContainsKey("description") ? args["description"].ToString() : null,
                Icon = args.ContainsKey("icon") ? args["icon"].ToString() : null,
                Attributes = args.ContainsKey("attributes") && args["attributes"] is Dictionary<string, object> attributes
                    ? attributes
                    : new Dictionary<string, object>(),
                Keywords = args.ContainsKey("keywords") && args["keywords"] is List<string> keywords
                    ? keywords
                    : new List<string>()
            };

            _registeredBlockTypes[name] = blockType;

            // Save to database
            dbContext.BlockTypes.Add(blockType);
            dbContext.SaveChanges();

            return blockType;
        }

        /// <summary>
        /// Unregisters a block type.
        /// </summary>
        public BlockType Unregister(string name, AppDbContext dbContext)
        {
            if (!_registeredBlockTypes.ContainsKey(name))
            {
                throw new InvalidOperationException($"Block type \"{name}\" is not registered.");
            }

            var unregisteredBlockType = _registeredBlockTypes[name];
            _registeredBlockTypes.Remove(name);

            // Remove from database
            var dbBlockType = dbContext.BlockTypes.FirstOrDefault(bt => bt.Name == name);
            if (dbBlockType != null)
            {
                dbContext.BlockTypes.Remove(dbBlockType);
                dbContext.SaveChanges();
            }

            return unregisteredBlockType;
        }

        /// <summary>
        /// Retrieves a registered block type by its name.
        /// </summary>
        public BlockType GetRegistered(string name)
        {
            return _registeredBlockTypes.ContainsKey(name) ? _registeredBlockTypes[name] : null;
        }

        /// <summary>
        /// Retrieves all registered block types.
        /// </summary>
        public IEnumerable<BlockType> GetAllRegistered()
        {
            return _registeredBlockTypes.Values;
        }

        /// <summary>
        /// Checks if a block type is registered.
        /// </summary>
        public bool IsRegistered(string name)
        {
            return _registeredBlockTypes.ContainsKey(name);
        }

        /// <summary>
        /// Utility method to retrieve the main instance of the class.
        /// </summary>
        public static BlockTypeRegistry GetInstance()
        {
            return _instance ??= new BlockTypeRegistry();
        }
    }
}