using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class UserModel
    {
        public string loginName { get; set; }

        public int id { get; set; }

        public string pwd { get; set; }

        public string phone { get; set; }
        public string zhifubao { get; set; }
        public string fatherphone { get; set; }
        public double allmoney { get; set; }
        public double nowmoney { get; set; }
        public double allsonmoney { get; set; }
        public double nowsonmoney { get; set; }
        public double sonnums { get; set; }
        public string qq { get; set; }
        public string remark1 { get; set; }
        public string remark2 { get; set; }
        public bool isused { get; set; }
        public string deviceid { get; set; }

        public double mymoney { get; set; }
        public string key {get;set;}

        public string timespan { get; set; }
    }
}