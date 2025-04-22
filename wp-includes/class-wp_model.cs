using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WordPress.Core.Models
{
    public class QueryVariable
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}