using System;
using System.ComponentModel.DataAnnotations;

namespace Requests.Core.Models
{
    public class RequestLog
    {
        [Key]
        public int Id { get; set; }

        public string Method { get; set; }
        public string Url { get; set; }
        public string Response { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
    }
}