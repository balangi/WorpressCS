using System;
using System.ComponentModel.DataAnnotations;

namespace POP3.Core.Models
{
    public class Pop3Log
    {
        [Key]
        public int Id { get; set; }

        public string Command { get; set; }
        public string Response { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
    }
}