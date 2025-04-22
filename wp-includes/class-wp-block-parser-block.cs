using System;
using System.Collections.Generic;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockParser
{
    public class BlockParserBlock
    {
        /// <summary>
        /// Name of the block.
        /// </summary>
        public string BlockName { get; set; }

        /// <summary>
        /// Optional set of attributes from block comment delimiters.
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// List of inner blocks (of this same class).
        /// </summary>
        public List<BlockParserBlock> InnerBlocks { get; set; } = new List<BlockParserBlock>();

        /// <summary>
        /// Resultant HTML from inside block comment delimiters after removing inner blocks.
        /// </summary>
        public string InnerHtml { get; set; }

        /// <summary>
        /// List of string fragments and null markers where inner blocks were found.
        /// </summary>
        public List<string> InnerContent { get; set; } = new List<string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the block.</param>
        /// <param name="attrs">Optional set of attributes from block comment delimiters.</param>
        /// <param name="innerBlocks">List of inner blocks.</param>
        /// <param name="innerHtml">Resultant HTML from inside block comment delimiters.</param>
        /// <param name="innerContent">List of string fragments and null markers.</param>
        public BlockParserBlock(
            string name,
            Dictionary<string, object> attrs,
            List<BlockParserBlock> innerBlocks,
            string innerHtml,
            List<string> innerContent)
        {
            BlockName = name;
            Attributes = attrs ?? new Dictionary<string, object>();
            InnerBlocks = innerBlocks ?? new List<BlockParserBlock>();
            InnerHtml = innerHtml ?? string.Empty;
            InnerContent = innerContent ?? new List<string>();
        }

        /// <summary>
        /// Saves the block to the database using EF Core.
        /// </summary>
        public void Save(AppDbContext dbContext)
        {
            var block = new Block
            {
                BlockName = BlockName,
                Attributes = Attributes,
                InnerHtml = InnerHtml,
                InnerContent = InnerContent
            };

            foreach (var innerBlock in InnerBlocks)
            {
                block.InnerBlocks.Add(new Block
                {
                    BlockName = innerBlock.BlockName,
                    Attributes = innerBlock.Attributes,
                    InnerHtml = innerBlock.InnerHtml,
                    InnerContent = innerBlock.InnerContent
                });
            }

            dbContext.Blocks.Add(block);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Loads a block from the database using EF Core.
        /// </summary>
        public static BlockParserBlock Load(int id, AppDbContext dbContext)
        {
            var block = dbContext.Blocks
                .Include(b => b.InnerBlocks)
                .FirstOrDefault(b => b.Id == id);

            if (block == null)
            {
                throw new InvalidOperationException("Block not found.");
            }

            return new BlockParserBlock(
                block.BlockName,
                block.Attributes,
                block.InnerBlocks.Select(innerBlock => Load(innerBlock.Id, dbContext)).ToList(),
                block.InnerHtml,
                block.InnerContent
            );
        }
    }
}