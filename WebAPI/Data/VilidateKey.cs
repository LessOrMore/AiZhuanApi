using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebAPI.Data
{
    public class VilidateKey
    {
        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="reson"></param>
        /// <returns></returns>
        public static bool CheckKey(Models.UserModel user,ref string reason)
        {
            try
            {
                string myKey = ConfigurationManager.AppSettings["myKey"].ToString().Trim();

                string keyStr = myKey + user.timespan + "ai1zhuan2";
                if (MD5Helper.GetMD5(keyStr, 2) != user.key)
                {
                    reason = "校验失败";
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                reason = e.Message;
                return false;
            }
        }

        public static bool CheckKey(string key, string timespan, ref string reason)
        {
            try
            {
                string myKey = ConfigurationManager.AppSettings["myKey"].ToString().Trim();

                string keyStr = myKey + timespan + "ai1zhuan2";
                if (MD5Helper.GetMD5(keyStr, 2) != key)
                {
                    reason = "校验失败";
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                reason = e.Message;
                return false;
            }
        }
    }
}