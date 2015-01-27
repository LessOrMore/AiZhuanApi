using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class AdminModel
    {
        public string user_id { get; set; }
        public string user_account { get; set; }
        public string user_pwd { get; set; }
        public string mobile { get; set; }
        public string last_login_time { get; set; }
        public  string last_login_ip {get;set;}
        public string real_name{get;set;}

        public string verifyCode { get; set; }

    }
}