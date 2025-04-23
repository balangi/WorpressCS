using System;
using System.Collections.Generic;
using System.Linq;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockSupports
{
    public class BlockSupports
    {
        /// <summary>
        /// Configuration for block supports.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, object>> _blockSupports = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Tracks the current block being rendered.
        /// </summary>
        public static Dictionary<string, object> BlockToRender { get; set; }

        /// <summary>
        /// Container for the main instance of the class.
        /// </summary>
        private static BlockSupports _instance;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private BlockSupports() { }

        /// <summary>
        /// Utility method to retrieve the main instance of the class.
        /// </summary>
        public static BlockSupports GetInstance()
        {
            return _instance ??= new BlockSupports();
        }

        /// <summary>
        /// Initializes the block supports. It registers the block support attributes.
        /// </summary>
        public static void Init(AppDbContext dbContext)
        {
            var instance = GetInstance();
            instance.RegisterAttributes(dbContext);
        }

        /// <summary>
        /// Registers a block support.
        /// </summary>
        public void Register(string blockSupportName, Dictionary<string, object> blockSupportConfig, AppDbContext dbContext)
        {
            if (string.IsNullOrEmpty(blockSupportName))
            {
                throw new ArgumentException("Block support name must be a non-empty string.");
            }

            var config = new Dictionary<string, object>(blockSupportConfig)
            {
                { "name", blockSupportName }
            };

            _blockSupports[blockSupportName] = config;

            // Save to database
            var blockSupport = new BlockSupport
            {
                Name = blockSupportName,
                Config = config
            };

            dbContext.BlockSupports.Add(blockSupport);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Generates an array of HTML attributes by applying all supported features to the given block.
        /// </summary>
        public Dictionary<string, string> ApplyBlockSupports(AppDbContext dbContext)
        {
            if (BlockToRender == null || !BlockToRender.ContainsKey("blockName"))
            {
                return new Dictionary<string, string>();
            }

            var blockName = BlockToRender["blockName"].ToString();
            var blockType = dbContext.BlockTypes.FirstOrDefault(bt => bt.Name == blockName);

            if (blockType == null)
            {
                return new Dictionary<string, string>();
            }

            var blockAttributes = BlockToRender.ContainsKey("attrs") && BlockToRender["attrs"] is Dictionary<string, object> attrs
                ? PrepareAttributesForRender(attrs, blockType)
                : new Dictionary<string, object>();

            var output = new Dictionary<string, string>();

            foreach (var blockSupportConfig in _blockSupports.Values)
            {
                if (!blockSupportConfig.ContainsKey("apply") || !(blockSupportConfig["apply"] is Func<Dictionary<string, object>, Dictionary<string, object>, Dictionary<string, string>> applyFunc))
                {
                    continue;
                }

                var newAttributes = applyFunc(blockType.Attributes, blockAttributes);

                if (newAttributes != null && newAttributes.Any())
                {
                    foreach (var attribute in newAttributes)
                    {
                        if (!output.ContainsKey(attribute.Key))
                        {
                            output[attribute.Key] = attribute.Value;
                        }
                        else
                        {
                            output[attribute.Key] += $" {attribute.Value}";
                        }
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Registers the block attributes required by the different block supports.
        /// </summary>
        private void RegisterAttributes(AppDbContext dbContext)
        {
            var registeredBlockTypes = dbContext.BlockTypes.ToList();

            foreach (var blockType in registeredBlockTypes)
            {
                if (blockType.Attributes == null)
                {
                    blockType.Attributes = new Dictionary<string, object>();
                }

                foreach (var blockSupportConfig in _blockSupports.Values)
                {
                    if (!blockSupportConfig.ContainsKey("registerAttribute") || !(blockSupportConfig["registerAttribute"] is Action<Dictionary<string, object>> registerAttributeAction))
                    {
                        continue;
                    }

                    registerAttributeAction(blockType.Attributes);
                }

                // Update block type in database
                dbContext.BlockTypes.Update(blockType);
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Prepares attributes for rendering.
        /// </summary>
        private Dictionary<string, object> PrepareAttributesForRender(Dictionary<string, object> attrs, dynamic blockType)
        {
            // Implement logic to prepare attributes for rendering
            return attrs;
        }
    }
}