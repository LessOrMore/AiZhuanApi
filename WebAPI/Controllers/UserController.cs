using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Data;
namespace WebAPI.Controllers
{
    public class UserController : ApiController
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>返回{reslt:true,reason:""}</returns>
        [HttpPost]
        public object Regist(WebAPI.Models.UserModel userInfo)
        {
            bool result = false;
            string reason = string.Empty;
            if (userInfo == null)
            {
                reason = "参数为空";
                var returnData = new { result = result, reason = reason };
                return returnData;
            }

            result = UserData.Instance.RegistUser(userInfo, ref reason);
            return new { result = result, reason = reason };
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userInfo">提供用户数据</param>
        /// <returns>{reslt:true,reason:""}</returns>
        [HttpPost]
        public object Login(WebAPI.Models.UserModel userInfo)
        {
            bool result = false;
            string reason = string.Empty;
            if (userInfo == null)
            {
                reason = "参数为空";
                var returnData = new { result = result, reason = reason };
                return returnData;
            }
            int type = -1;
            
            result = UserData.Instance.Login(ref userInfo, ref reason,ref type);
            if (!result)
            {
                return new { result = result, reason = reason };
            }
            return new { result = result, reason = reason, type = type, userid = userInfo.id };
        }

        /// <summary>
        /// 增加流量币接口
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddMoney(Models.UserModel userInfo)
        {
            if (userInfo == null)
            {
                return new { result=false,reason="参数解析错误" };
            }
            string reason = string.Empty;
            bool result = UserData.Instance.AddMoney(userInfo, ref reason);
            return new { result = result, reason = reason};
        }

        /// <summary>
        /// 找回密码接口
        /// </summary>
        /// <param name="phone">注册手机号</param>
        /// <param name="qq">注册QQ</param>
        /// <returns></returns>
        [HttpGet]
        public object FindPwd(string phone, string qq)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return new { result = false, reason = "手机号不正确" };
            }
            string errMsg = string.Empty;
            Models.UserModel userModel = UserData.Instance.GetUser(phone, ref errMsg);
            if(!string.IsNullOrEmpty(errMsg))
            {
                return new { result = false, reason = "根据提供的手机号查询"+errMsg };
            }

            if (userModel.qq!=qq)
            {
                return new { result = false, reason = "qq号码不正确" };
            }


            string reason = string.Empty;
            Random ran = new Random();
            var pwd = ran.Next(100000, 1000000).ToString();
            var md5pwd = MD5Helper.GetMD5(pwd,2);
            //更新密码
            bool result = UserData.Instance.UpdatePwd(phone, md5pwd, ref reason);
            if (!string.IsNullOrEmpty(reason))
            {
                return new { result = result, reason = reason};
            }

            try
            {
                //发送邮件
                string senderTitle = System.Configuration.ConfigurationManager.AppSettings["SenderTitle"].ToString();
                MailHelper.Send(qq + "@qq.com", "找回密码", string.Format(senderTitle+"提示您:您的密码重置成功,新密码为:{0},请妥善保存", pwd));
            }
            catch(Exception e)
            {
                return new { result = false, reason = e.Message };
            }

            return new { result = result, reason = reason };
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="phone">用户手机号</param>
        /// <returns></returns>
        [HttpGet]
        public object GetUserModel(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return new { result = false, reason = "电话号码为空" };
            }
            
            string errMsg = string.Empty;
            Models.UserModel user = UserData.Instance.GetUser(phone, ref errMsg);

            if (!string.IsNullOrEmpty(errMsg))
            {
                return new { result = false, reason = errMsg };
            }
            return new { phone = user.phone, fatherphone = user.fatherphone, allmoney = user.allmoney, nowmoney = user.nowmoney, allsonmoney = user.allsonmoney, nowsonmoney = user.nowsonmoney, sonnums =user.sonnums};

        }
    }
}