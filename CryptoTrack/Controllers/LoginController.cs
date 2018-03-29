using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoTrack.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        /*
            Documentation:
            This function check whether login form was submited, if yes it generates timestamp and hash from username and
            create login token. Search database for username and then compare pwd. Create record in DB with token and start
            of login session. Save login token to server session variable. Return view which is welcome page after login - 
            view should have different navbar (with logout button instaed).
        */
        public ActionResult Login() {

            ViewBag.Submitted = false;

            if (HttpContext.Request.RequestType == "POST") {

                ViewBag.Submitted = true;

                string name = Request.Form["username"];
                string pwd = Request.Form["password"];

                string hash = "";
                try
                {
                    hash = Utils.Helpers.MD5Hash(name);
                }
                catch(Exception e)
                {
                    ViewBag.Message = e.Message;
                }

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string token = hash + ":" + timestamp;

                //ViewBag.Message = "Logged in : " + token;

            }
            return View();
        }
    }
}