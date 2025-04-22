using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WordPress.Core.Models
{
    public class Block
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
        public List<Block> InnerBlocks { get; set; } = new List<Block>();
        public string InnerHtml { get; set; }
        public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();
    }
}