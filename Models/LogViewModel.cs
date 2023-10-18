using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myhw.Models
{
    public class LogViewModel
    {      /// <summary>
           /// 使用者名稱
           /// </summary>


        public string Username { get; set; }
        /// <summary>
        /// 使用者密碼
        /// </summary>

        public string Password { get; set; }
        /// <summary>
        /// 是否記得我
        /// </summary>
        public bool RememberMe { get; set; }

    }
}