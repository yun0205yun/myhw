using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myhw.Models
{
    public class MessageDataModel
    {
        public int UserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 留言
        /// </summary>
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

         
    }
}