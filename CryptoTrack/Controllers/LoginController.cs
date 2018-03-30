using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                string timestampDB = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                string token = hash + ":" + timestamp;

                Session["LoginToken"] = token;
                
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnString"].ConnectionString);
                connection.Open();
                string query = "INSERT INTO Logins (LoginToken,Start) VALUES (@Token,@StartDate)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Token", token);
                cmd.Parameters.AddWithValue("@StartDate", timestampDB);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    ViewBag.Message = e.Message;
                }
                connection.Close();

            }
            return View();
        }
    }
}