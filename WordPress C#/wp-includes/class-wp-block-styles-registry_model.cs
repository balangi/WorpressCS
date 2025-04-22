using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class BlockStyle
    {
        public int Id { get; set; }
        public string BlockName { get; set; }
        public string StyleName { get; set; }
        public string Label { get; set; }
        public string InlineStyle { get; set; }
        public string StyleHandle { get; set; }
        public bool IsDefault { get; set; }
        public Dictionary<string, object> StyleData { get; set; } = new Dictionary<string, object>();
    }
}