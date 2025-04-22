using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class Block
    {
        public int Id { get; set; }
        public string BlockName { get; set; } // Name of block
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>(); // Optional attributes
        public List<Block> InnerBlocks { get; set; } = new List<Block>(); // List of inner blocks
        public string InnerHtml { get; set; } // Resultant HTML from inside block comment delimiters
        public List<string> InnerContent { get; set; } = new List<string>(); // List of string fragments and null markers
    }
}