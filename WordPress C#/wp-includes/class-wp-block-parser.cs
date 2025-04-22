using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockParser
{
    public class BlockParser
    {
        /// <summary>
        /// Input document being parsed.
        /// </summary>
        public string Document { get; private set; }

        /// <summary>
        /// Tracks parsing progress through the document.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// List of parsed blocks.
        /// </summary>
        public List<Block> Output { get; private set; } = new List<Block>();

        /// <summary>
        /// Stack for tracking nested blocks.
        /// </summary>
        private readonly Stack<BlockParserFrame> _stack = new Stack<BlockParserFrame>();

        private readonly AppDbContext _dbContext;

        public BlockParser(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Parses the input document and constructs a list of parsed block objects.
        /// </summary>
        public void Parse(string document)
        {
            Document = document;
            Offset = 0;
            Output.Clear();
            _stack.Clear();

            while (Offset < Document.Length)
            {
                var token = NextToken();
                if (token == null)
                {
                    break;
                }

                ProcessToken(token);
            }

            // Flush any remaining freeform content
            AddFreeform();
        }

        /// <summary>
        /// Finds the next valid token to parse.
        /// </summary>
        private (string Type, string Name, Dictionary<string, object> Attributes, int StartOffset, int Length)? NextToken()
        {
            var regex = new Regex(@"<!--\s+(?P<closer>/)?wp:(?P<namespace>[a-z][a-z0-9_-]*\/)?(?P<name>[a-z][a-z0-9_-]*)\s+(?P<attrs>{(?:(?:[^}]+|}+(?=})|(?!}\s+\/?-->).)*+)?}\s+)?(?P<void>/)?-->", RegexOptions.Singleline);
            var match = regex.Match(Document, Offset);

            if (!match.Success)
            {
                return null;
            }

            var type = match.Groups["closer"].Success ? "block-closer" : "block-opener";
            var name = match.Groups["name"].Value;
            var attributes = ParseAttributes(match.Groups["attrs"].Value);
            var startOffset = match.Index;
            var length = match.Length;

            Offset = startOffset + length;

            return (type, name, attributes, startOffset, length);
        }

        /// <summary>
        /// Processes a parsed token.
        /// </summary>
        private void ProcessToken((string Type, string Name, Dictionary<string, object> Attributes, int StartOffset, int Length) token)
        {
            switch (token.Type)
            {
                case "block-opener":
                    HandleBlockOpener(token.Name, token.Attributes, token.StartOffset, token.Length);
                    break;

                case "block-closer":
                    HandleBlockCloser(token.Name, token.StartOffset, token.Length);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown token type: {token.Type}");
            }
        }

        /// <summary>
        /// Handles a block opener token.
        /// </summary>
        private void HandleBlockOpener(string name, Dictionary<string, object> attributes, int startOffset, int length)
        {
            var block = new Block
            {
                Name = name,
                Attributes = attributes,
                InnerHtml = "",
                InnerContent = new List<string>()
            };

            if (_stack.Count > 0)
            {
                var parent = _stack.Peek();
                parent.Block.InnerBlocks.Add(block);
            }
            else
            {
                Output.Add(block);
            }

            _stack.Push(new BlockParserFrame(block, startOffset, length));
        }

        /// <summary>
        /// Handles a block closer token.
        /// </summary>
        private void HandleBlockCloser(string name, int endOffset, int length)
        {
            if (_stack.Count == 0)
            {
                throw new InvalidOperationException("Unexpected block closer without an opener.");
            }

            var frame = _stack.Pop();
            var block = frame.Block;

            // Append any remaining inner HTML
            var html = Document.Substring(frame.PrevOffset, endOffset - frame.PrevOffset).Trim();
            if (!string.IsNullOrEmpty(html))
            {
                block.InnerHtml += html;
                block.InnerContent.Add(html);
            }

            // If stack is empty, this is a top-level block
            if (_stack.Count == 0)
            {
                SaveBlockToDatabase(block);
            }
        }

        /// <summary>
        /// Adds freeform content to the output.
        /// </summary>
        private void AddFreeform(int? length = null)
        {
            length ??= Document.Length - Offset;
            if (length <= 0)
            {
                return;
            }

            var freeformContent = Document.Substring(Offset, length.Value);
            Output.Add(new Block
            {
                Name = null,
                Attributes = new Dictionary<string, object>(),
                InnerHtml = freeformContent,
                InnerContent = new List<string> { freeformContent }
            });

            Offset += length.Value;
        }

        /// <summary>
        /// Parses block attributes from a JSON-like string.
        /// </summary>
        private Dictionary<string, object> ParseAttributes(string attrs)
        {
            if (string.IsNullOrEmpty(attrs))
            {
                return new Dictionary<string, object>();
            }

            // Simulate parsing attributes (in a real scenario, use a JSON parser)
            return new Dictionary<string, object>
            {
                { "example", "value" }
            };
        }

        /// <summary>
        /// Saves a block to the database.
        /// </summary>
        private void SaveBlockToDatabase(Block block)
        {
            _dbContext.Blocks.Add(block);
            _dbContext.SaveChanges();
        }
    }

    /// <summary>
    /// Represents a frame in the block parsing stack.
    /// </summary>
    public class BlockParserFrame
    {
        public Block Block { get; }
        public int PrevOffset { get; }
        public int TokenLength { get; }

        public BlockParserFrame(Block block, int prevOffset, int tokenLength)
        {
            Block = block;
            PrevOffset = prevOffset;
            TokenLength = tokenLength;
        }
    }
}