using System;
using System.Collections.Generic;
using System.Linq;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockTypes
{
    public class BlockTypeManager
    {
        /// <summary>
        /// Block API version.
        /// </summary>
        public int ApiVersion { get; set; } = 1;

        /// <summary>
        /// Block type key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Human-readable block type label.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Block type category classification.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Block type description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Block type icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Block type attributes.
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Contexts used by the block type.
        /// </summary>
        public List<string> UsesContext { get; set; } = new List<string>();

        /// <summary>
        /// Contexts provided by the block type.
        /// </summary>
        public List<string> ProvidesContext { get; set; } = new List<string>();

        /// <summary>
        /// Editor-only script handles.
        /// </summary>
        public List<string> EditorScriptHandles { get; set; } = new List<string>();

        /// <summary>
        /// Front-end and editor script handles.
        /// </summary>
        public List<string> ScriptHandles { get; set; } = new List<string>();

        /// <summary>
        /// Front-end only script handles.
        /// </summary>
        public List<string> ViewScriptHandles { get; set; } = new List<string>();

        /// <summary>
        /// Editor-only style handles.
        /// </summary>
        public List<string> EditorStyleHandles { get; set; } = new List<string>();

        /// <summary>
        /// Front-end and editor style handles.
        /// </summary>
        public List<string> StyleHandles { get; set; } = new List<string>();

        /// <summary>
        /// Front-end only style handles.
        /// </summary>
        public List<string> ViewStyleHandles { get; set; } = new List<string>();

        /// <summary>
        /// Registers a block type.
        /// </summary>
        public BlockType Register(string blockType, Dictionary<string, object> args, AppDbContext dbContext)
        {
            if (string.IsNullOrEmpty(blockType))
            {
                throw new ArgumentException("Block type name must be a non-empty string.");
            }

            var block = new BlockType
            {
                Name = blockType,
                Title = args.ContainsKey("title") ? args["title"].ToString() : string.Empty,
                Category = args.ContainsKey("category") ? args["category"].ToString() : null,
                Description = args.ContainsKey("description") ? args["description"].ToString() : null,
                Icon = args.ContainsKey("icon") ? args["icon"].ToString() : null,
                Attributes = args.ContainsKey("attributes") && args["attributes"] is Dictionary<string, object> attributes
                    ? attributes
                    : new Dictionary<string, object>(),
                UsesContext = args.ContainsKey("uses_context") && args["uses_context"] is List<string> usesContext
                    ? usesContext
                    : new List<string>(),
                ProvidesContext = args.ContainsKey("provides_context") && args["provides_context"] is List<string> providesContext
                    ? providesContext
                    : new List<string>(),
                EditorScriptHandles = args.ContainsKey("editor_script_handles") && args["editor_script_handles"] is List<string> editorScripts
                    ? editorScripts
                    : new List<string>(),
                ScriptHandles = args.ContainsKey("script_handles") && args["script_handles"] is List<string> scripts
                    ? scripts
                    : new List<string>(),
                ViewScriptHandles = args.ContainsKey("view_script_handles") && args["view_script_handles"] is List<string> viewScripts
                    ? viewScripts
                    : new List<string>(),
                EditorStyleHandles = args.ContainsKey("editor_style_handles") && args["editor_style_handles"] is List<string> editorStyles
                    ? editorStyles
                    : new List<string>(),
                StyleHandles = args.ContainsKey("style_handles") && args["style_handles"] is List<string> styles
                    ? styles
                    : new List<string>(),
                ViewStyleHandles = args.ContainsKey("view_style_handles") && args["view_style_handles"] is List<string> viewStyles
                    ? viewStyles
                    : new List<string>()
            };

            // Add global attributes if not already defined
            foreach (var globalAttribute in GlobalAttributes)
            {
                if (!block.Attributes.ContainsKey(globalAttribute.Key))
                {
                    block.Attributes[globalAttribute.Key] = globalAttribute.Value;
                }
            }

            // Save to database
            dbContext.BlockTypes.Add(block);
            dbContext.SaveChanges();

            return block;
        }

        /// <summary>
        /// Retrieves a registered block type by its name.
        /// </summary>
        public BlockType GetRegistered(string blockTypeName, AppDbContext dbContext)
        {
            return dbContext.BlockTypes.FirstOrDefault(bt => bt.Name == blockTypeName);
        }

        /// <summary>
        /// Retrieves all registered block types.
        /// </summary>
        public IEnumerable<BlockType> GetAllRegistered(AppDbContext dbContext)
        {
            return dbContext.BlockTypes.ToList();
        }

        /// <summary>
        /// Unregisters a block type.
        /// </summary>
        public bool Unregister(string blockTypeName, AppDbContext dbContext)
        {
            var blockType = dbContext.BlockTypes.FirstOrDefault(bt => bt.Name == blockTypeName);

            if (blockType == null)
            {
                throw new InvalidOperationException($"Block type \"{blockTypeName}\" is not registered.");
            }

            dbContext.BlockTypes.Remove(blockType);
            dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// Global attributes supported by every block.
        /// </summary>
        private static readonly Dictionary<string, object> GlobalAttributes = new Dictionary<string, object>
        {
            { "lock", new { type = "object" } },
            { "metadata", new { type = "object" } }
        };
    }
}