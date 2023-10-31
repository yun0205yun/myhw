using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions;

namespace myhw.Models
{
    
    public class MessageDataModel
    {
        public int ContentId { get; set; }
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
        public int TotalMessages { get; internal set; }
    }
}