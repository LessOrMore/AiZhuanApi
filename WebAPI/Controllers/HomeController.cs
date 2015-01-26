using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AdminLogin(WebAPI.Models.AdminModel adminInfo)
        {
            JsonResult resultJson = new JsonResult();
            if (adminInfo == null)
            {
                var returnData = new { result = false, reason = "参数不正确" };
                resultJson.Data = returnData;
                return resultJson;
            }
            string reason = string.Empty;
            Models.AdminModel currentAdmin = WebAPI.Data.UserData.Instance.AdminLogin(adminInfo, ref reason);
            if (!string.IsNullOrEmpty(reason))
            {
                var data = new { result = true, reason = reason };
                resultJson.Data = data;
                return resultJson;

            }

            Session["Admin"] = currentAdmin;

            var successData = new { result = true, reason = reason };
            resultJson.Data = successData;
            return resultJson;
        }
        public JsonResult SetMoney()
        {
            string phone = Request.Params["phone"].ToString();
            double nowMoney = Convert.ToDouble(Request.Params["nowMoney"].ToString());
            double nowSonMoney = Convert.ToDouble(Request.Params["nowSonMoney"].ToString());
            JsonResult returnJson = new JsonResult();
            returnJson.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (string.IsNullOrEmpty(phone))
            {
                var returnData = new { result = false, reason = "电话号码为空" };
                returnJson.Data = returnData;
                return returnJson;
            }
            string reason = string.Empty;
            bool result = WebAPI.Data.UserData.Instance.SetMoney(phone, nowMoney, nowSonMoney, ref reason);
            if (result)
            {
                reason = "设置成功"; 
            }
            var data = new { result = result, reason = reason };
            returnJson.Data = data;
            return returnJson;
        }

        public ActionResult Main()
        {
            Models.AdminModel admin = Session["Admin"] as Models.AdminModel;
            if (admin == null)
            {
                Response.Redirect("/Home/Index");
            }
            return View();
        }

        public JsonResult GetUserDetails()
        {
            Dictionary<string, string> param = this.GetPageParam();
            int count=0;
            string errMsg=string.Empty;

            JsonResult resultJson = new JsonResult();
            resultJson.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            List<Models.UserModel> modelList = WebAPI.Data.UserData.Instance.GetUserList(param, ref count, ref errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                var errData = new { total = 0, rows = modelList };
                resultJson.Data = errData;
                return resultJson;
            }

            var userData = new { total = count, rows = modelList };
            resultJson.Data = userData;
            return resultJson;

        }

        private Dictionary<string, string> GetPageParam()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (Request.Params["phone"] != null && !string.IsNullOrEmpty(Request.Params["phone"].ToString())) 
            {
                param.Add("phone", Request.Params["phone"].ToString());
            }

            string offset = Request.Params["offset"];
            string limit = Request.Params["limit"];
            if (!string.IsNullOrEmpty(offset) && !string.IsNullOrEmpty(limit))
            {
                int startRow = Convert.ToInt32(offset) * Convert.ToInt32(limit) + 1;
                param.Add("startRow", startRow.ToString());
                int endRow = (Convert.ToInt32(offset) + 1) * Convert.ToInt32(limit);
                param.Add("endRow", endRow.ToString());
            }
            return param;
        }


        public ActionResult Test()
        {
            return View();
        }
    }
}
