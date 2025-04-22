using System;
using System.Collections.Generic;
using System.Linq;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockStyles
{
    public class BlockStylesRegistry
    {
        /// <summary>
        /// Registered block styles dictionary.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, BlockStyle>> _registeredBlockStyles = new Dictionary<string, Dictionary<string, BlockStyle>>();

        /// <summary>
        /// Container for the main instance of the class.
        /// </summary>
        private static BlockStylesRegistry _instance;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private BlockStylesRegistry() { }

        /// <summary>
        /// Registers a block style for the given block type.
        /// </summary>
        public bool Register(string blockName, Dictionary<string, object> styleProperties, AppDbContext dbContext)
        {
            if (string.IsNullOrEmpty(blockName))
            {
                throw new ArgumentException("Block name must be a string.");
            }

            if (!styleProperties.ContainsKey("name") || !(styleProperties["name"] is string styleName) || string.IsNullOrWhiteSpace(styleName))
            {
                throw new ArgumentException("Block style name must be a non-empty string.");
            }

            if (styleName.Contains(" "))
            {
                throw new ArgumentException("Block style name must not contain any spaces.");
            }

            var label = styleProperties.ContainsKey("label") && !string.IsNullOrEmpty(styleProperties["label"].ToString())
                ? styleProperties["label"].ToString()
                : styleName;

            var inlineStyle = styleProperties.ContainsKey("inline_style") ? styleProperties["inline_style"]?.ToString() : null;
            var styleHandle = styleProperties.ContainsKey("style_handle") ? styleProperties["style_handle"]?.ToString() : null;
            var isDefault = styleProperties.ContainsKey("is_default") && styleProperties["is_default"] is bool defaultValue ? defaultValue : false;
            var styleData = styleProperties.ContainsKey("style_data") && styleProperties["style_data"] is Dictionary<string, object> data ? data : new Dictionary<string, object>();

            var blockStyle = new BlockStyle
            {
                BlockName = blockName,
                StyleName = styleName,
                Label = label,
                InlineStyle = inlineStyle,
                StyleHandle = styleHandle,
                IsDefault = isDefault,
                StyleData = styleData
            };

            if (!_registeredBlockStyles.ContainsKey(blockName))
            {
                _registeredBlockStyles[blockName] = new Dictionary<string, BlockStyle>();
            }

            _registeredBlockStyles[blockName][styleName] = blockStyle;

            // Save to database
            dbContext.BlockStyles.Add(blockStyle);
            dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// Unregisters a block style for the given block type.
        /// </summary>
        public bool Unregister(string blockName, string styleName, AppDbContext dbContext)
        {
            if (!_registeredBlockStyles.ContainsKey(blockName) || !_registeredBlockStyles[blockName].ContainsKey(styleName))
            {
                throw new KeyNotFoundException($"Block \"{blockName}\" does not contain a style named \"{styleName}\".");
            }

            _registeredBlockStyles[blockName].Remove(styleName);

            // Remove from database
            var blockStyle = dbContext.BlockStyles.FirstOrDefault(bs => bs.BlockName == blockName && bs.StyleName == styleName);
            if (blockStyle != null)
            {
                dbContext.BlockStyles.Remove(blockStyle);
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// Retrieves the properties of a registered block style for the given block type.
        /// </summary>
        public BlockStyle GetRegistered(string blockName, string styleName)
        {
            return _registeredBlockStyles.ContainsKey(blockName) && _registeredBlockStyles[blockName].ContainsKey(styleName)
                ? _registeredBlockStyles[blockName][styleName]
                : null;
        }

        /// <summary>
        /// Retrieves all registered block styles.
        /// </summary>
        public IEnumerable<BlockStyle> GetAllRegistered()
        {
            return _registeredBlockStyles.Values.SelectMany(styles => styles.Values);
        }

        /// <summary>
        /// Retrieves registered block styles for a specific block type.
        /// </summary>
        public IEnumerable<BlockStyle> GetRegisteredStylesForBlock(string blockName)
        {
            return _registeredBlockStyles.ContainsKey(blockName) ? _registeredBlockStyles[blockName].Values : Enumerable.Empty<BlockStyle>();
        }

        /// <summary>
        /// Checks if a block style is registered for the given block type.
        /// </summary>
        public bool IsRegistered(string blockName, string styleName)
        {
            return _registeredBlockStyles.ContainsKey(blockName) && _registeredBlockStyles[blockName].ContainsKey(styleName);
        }

        /// <summary>
        /// Utility method to retrieve the main instance of the class.
        /// </summary>
        public static BlockStylesRegistry GetInstance()
        {
            return _instance ??= new BlockStylesRegistry();
        }
    }
}