using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myhw.Models
{
    public class CreateModel
    {
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}