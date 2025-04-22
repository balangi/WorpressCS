using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.Blocks
{
    public class WPBlock
    {
        /// <summary>
        /// Original parsed array representation of block.
        /// </summary>
        public Dictionary<string, object> ParsedBlock { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Name of block.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Block type associated with the instance.
        /// </summary>
        public string BlockType { get; set; }

        /// <summary>
        /// Block context values.
        /// </summary>
        public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// All available context of the current hierarchy.
        /// </summary>
        private Dictionary<string, object> AvailableContext { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// List of inner blocks.
        /// </summary>
        public List<WPBlock> InnerBlocks { get; set; } = new List<WPBlock>();

        /// <summary>
        /// Resultant HTML from inside block comment delimiters after removing inner blocks.
        /// </summary>
        public string InnerHtml { get; set; }

        private readonly AppDbContext _dbContext;

        public WPBlock(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Renders the block content.
        /// </summary>
        public string Render()
        {
            var output = new StringBuilder();

            // Process inner blocks
            foreach (var innerBlock in InnerBlocks)
            {
                output.Append(innerBlock.Render());
            }

            // Append inner HTML
            output.Append(InnerHtml);

            return output.ToString();
        }

        /// <summary>
        /// Updates the context for the current block and its inner blocks.
        /// </summary>
        public void UpdateContext()
        {
            if (ParsedBlock.ContainsKey("context"))
            {
                Context = (Dictionary<string, object>)ParsedBlock["context"];
            }

            foreach (var innerBlock in InnerBlocks)
            {
                innerBlock.AvailableContext = new Dictionary<string, object>(AvailableContext);
                innerBlock.UpdateContext();
            }
        }

        /// <summary>
        /// Processes block bindings.
        /// </summary>
        private Dictionary<string, object> ProcessBlockBindings()
        {
            var computedAttributes = new Dictionary<string, object>();

            // Example: Process supported block attributes
            var supportedBlockAttributes = new Dictionary<string, List<string>>
            {
                { "core/paragraph", new List<string> { "content" } },
                { "core/heading", new List<string> { "content" } }
            };

            if (supportedBlockAttributes.ContainsKey(Name))
            {
                foreach (var attribute in supportedBlockAttributes[Name])
                {
                    if (ParsedBlock.ContainsKey(attribute))
                    {
                        computedAttributes[attribute] = ParsedBlock[attribute];
                    }
                }
            }

            return computedAttributes;
        }

        /// <summary>
        /// Replaces HTML attributes in the block content.
        /// </summary>
        private string ReplaceHtml(string content, string attributeName, object sourceValue)
        {
            return content.Replace($"{{{attributeName}}}", sourceValue.ToString());
        }

        /// <summary>
        /// Saves the block to the database.
        /// </summary>
        public void Save()
        {
            var block = new Block
            {
                Name = Name,
                Attributes = ParsedBlock,
                InnerHtml = InnerHtml,
                Context = Context
            };

            _dbContext.Blocks.Add(block);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Loads a block from the database.
        /// </summary>
        public static WPBlock Load(int id, AppDbContext dbContext)
        {
            var block = dbContext.Blocks.Include(b => b.InnerBlocks).FirstOrDefault(b => b.Id == id);
            if (block == null)
            {
                throw new InvalidOperationException("Block not found.");
            }

            return new WPBlock(dbContext)
            {
                Name = block.Name,
                ParsedBlock = block.Attributes,
                InnerHtml = block.InnerHtml,
                Context = block.Context,
                InnerBlocks = block.InnerBlocks.Select(b => Load(b.Id, dbContext)).ToList()
            };
        }
    }
}