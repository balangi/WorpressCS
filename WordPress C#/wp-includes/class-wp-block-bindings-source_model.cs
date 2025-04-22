using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WordPress.Core.Models
{
    public class BlockBindingSource
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string GetValueCallback { get; set; } // Serialized as JSON or a reference to a delegate
        public List<string> UsesContext { get; set; } = new List<string>();
    }

    public class BlockBindingRegistry
    {
        public int Id { get; set; }
        public List<BlockBindingSource> Sources { get; set; } = new List<BlockBindingSource>();
    }
}