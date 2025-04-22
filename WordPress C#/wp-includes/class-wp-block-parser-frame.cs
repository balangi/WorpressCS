using System;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockParser
{
    public class BlockParserFrame
    {
        /// <summary>
        /// Full or partial block.
        /// </summary>
        public BlockParserBlock Block { get; private set; }

        /// <summary>
        /// Byte offset into document for start of parse token.
        /// </summary>
        public int TokenStart { get; private set; }

        /// <summary>
        /// Byte length of entire parse token string.
        /// </summary>
        public int TokenLength { get; private set; }

        /// <summary>
        /// Byte offset into document for after parse token ends.
        /// </summary>
        public int PrevOffset { get; private set; }

        /// <summary>
        /// Byte offset into document where leading HTML before token starts.
        /// </summary>
        public int? LeadingHtmlStart { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="block">Full or partial block.</param>
        /// <param name="tokenStart">Byte offset into document for start of parse token.</param>
        /// <param name="tokenLength">Byte length of entire parse token string.</param>
        /// <param name="prevOffset">Optional. Byte offset into document for after parse token ends.</param>
        /// <param name="leadingHtmlStart">Optional. Byte offset into document where leading HTML before token starts.</param>
        public BlockParserFrame(
            BlockParserBlock block,
            int tokenStart,
            int tokenLength,
            int? prevOffset = null,
            int? leadingHtmlStart = null)
        {
            Block = block ?? throw new ArgumentNullException(nameof(block));
            TokenStart = tokenStart;
            TokenLength = tokenLength;
            PrevOffset = prevOffset ?? (tokenStart + tokenLength);
            LeadingHtmlStart = leadingHtmlStart;
        }

        /// <summary>
        /// Saves the frame to the database using EF Core.
        /// </summary>
        public void Save(AppDbContext dbContext)
        {
            var frame = new Models.BlockParserFrame
            {
                BlockName = Block.BlockName,
                Attributes = Block.Attributes,
                TokenStart = TokenStart,
                TokenLength = TokenLength,
                PrevOffset = PrevOffset,
                LeadingHtmlStart = LeadingHtmlStart
            };

            dbContext.BlockParserFrames.Add(frame);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Loads a frame from the database using EF Core.
        /// </summary>
        public static BlockParserFrame Load(int id, AppDbContext dbContext)
        {
            var frame = dbContext.BlockParserFrames.FirstOrDefault(f => f.Id == id);

            if (frame == null)
            {
                throw new InvalidOperationException("Frame not found.");
            }

            var block = new BlockParserBlock(
                frame.BlockName,
                frame.Attributes,
                new List<BlockParserBlock>(),
                "",
                new List<string>()
            );

            return new BlockParserFrame(
                block,
                frame.TokenStart,
                frame.TokenLength,
                frame.PrevOffset,
                frame.LeadingHtmlStart
            );
        }
    }
}