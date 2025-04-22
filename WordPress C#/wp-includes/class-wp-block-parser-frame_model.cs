using System;

namespace WordPress.Core.Models
{
    public class BlockParserFrame
    {
        public int Id { get; set; }

        // Full or partial block
        public string BlockName { get; set; }
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        // Byte offset into document for start of parse token
        public int TokenStart { get; set; }

        // Byte length of entire parse token string
        public int TokenLength { get; set; }

        // Byte offset into document for after parse token ends
        public int PrevOffset { get; set; }

        // Byte offset into document where leading HTML before token starts
        public int? LeadingHtmlStart { get; set; }
    }
}