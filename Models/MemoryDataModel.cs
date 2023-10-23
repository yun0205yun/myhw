using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myhw.Models
{
    public class MemoryDataModel
    {
        public string Username { get; set; }
        /// <summary>
        /// 使用者密碼
        /// </summary>

        public string Password { get; set; }
        /// <summary>
        /// 是否記得我
        /// </summary>
        public bool RememberMe { get; set; }
        public bool IsLoginSuccessful { get; internal set; }
    }
}