using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WordPress.Core.Models
{
    public class AjaxResponse
    {
        [Key]
        public int Id { get; set; }
        public string Action { get; set; }
        public string What { get; set; }
        public string Position { get; set; }
        public string Data { get; set; }
        public Dictionary<string, string> Supplemental { get; set; } = new Dictionary<string, string>();
    }
}