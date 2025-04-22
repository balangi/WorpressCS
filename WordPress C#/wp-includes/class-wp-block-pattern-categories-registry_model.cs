using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class BlockPatternCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool IsRegisteredOutsideInit { get; set; }
    }
}