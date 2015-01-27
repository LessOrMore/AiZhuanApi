using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAPI.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/

        public ActionResult Index()
        {
            return Redirect("~/index.html");
        }


        /// <summary>
        /// 验证码方法
        /// </summary>
        
    }
}
