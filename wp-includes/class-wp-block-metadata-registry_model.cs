using System;
using System.Collections.Generic;

namespace WordPress.Core.Models
{
    public class BlockMetadataCollection
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    public class BlockMetadataFile
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Identifier { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}