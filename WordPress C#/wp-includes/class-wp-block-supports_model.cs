using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class BlockSupport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Config { get; set; } = new Dictionary<string, object>();
    }
}