using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class Block
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
        public List<Block> InnerBlocks { get; set; } = new List<Block>();
    }

    public class BlockList
    {
        public int Id { get; set; }
        public List<Block> Blocks { get; set; } = new List<Block>();
    }
}