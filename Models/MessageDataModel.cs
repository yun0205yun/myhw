using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions;

namespace myhw.Models
{
    [Table("Content")]
    public class MessageDataModel
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int ContentId { get; set; }

        [Required(ErrorMessage = "UserId 是必需的")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username 是必需的")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email 是必需的")]
        [EmailAddress(ErrorMessage = "無效的電子郵件地址")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Content 是必需的")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Timestamp 是必需的")]
        public DateTime Timestamp { get; set; }

        public int TotalMessages { get; internal set; }
        public IPagedList<MessageDataModel> Messages { get; set; }
    }
}