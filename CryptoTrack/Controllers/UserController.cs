using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CryptoTrack.Models;
using Newtonsoft.Json;

namespace CryptoTrack.Controllers
{
    public class UserController : Controller
    {
        // GET: Display all users (not direct connection - only accessable for those who know about the index page)
        public ActionResult Index()
        {
            var users = Models.User.GetUsers();
            return View(users);
        }

        public ActionResult Create()
        {
            ViewBag.Submitted = false;
            var created = false;

            if(HttpContext.Request.RequestType == "POST")
            {
                ViewBag.Submitted = true;
                
                var name = Request.Form["name"];
                var pwd1 = Request.Form["pwd1"];
                var pwd2 = Request.Form["pwd2"];

                if (pwd1 != pwd2 || name == "" || pwd1== "" || pwd2 == "")
                {
                    ViewBag.Message = "Your passwords does not match or you did not entered all information!";
                    return View();
                }
                else
                {
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnString"].ConnectionString);
                    string SQLquery = "SELECT * FROM Users";
                    SqlCommand cmd = new SqlCommand(SQLquery, connection);
                    User DBuser = new User();
                    connection.Open();

                    int UserCounter = 0;
                    using (SqlDataReader oReader = cmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            DBuser.ID = int.Parse(oReader["Id"].ToString());
                            DBuser.Name = oReader["Username"].ToString();
                            DBuser.Password = oReader["Password"].ToString();
                            if(DBuser.Name.Trim() == name)
                            {
                                ViewBag.Message = "User with this username already exists, please choose another name and try again";
                                return View();
                            }
                            UserCounter++;
                        }
                    }

                    User NewUser = new User()
                    {
                        ID = UserCounter + 1,
                        Name = name,
                        Password = pwd1
                    };

                    try
                    {
                        SQLquery = "INSERT INTO Users (Id,Username,Password) VALUES (@ID,@Username,@Password)";
                        cmd = new SqlCommand(SQLquery, connection);
                        cmd.Parameters.AddWithValue("@ID", NewUser.ID);
                        cmd.Parameters.AddWithValue("@Username", NewUser.Name);
                        cmd.Parameters.AddWithValue("@Password", NewUser.Password);
                        cmd.ExecuteNonQuery();
                    }
                    catch(Exception e) {
                        ViewBag.Message = "Exception : " + e.Message;
                    }


                    connection.Close();
                    created = true;
                }
            }

            if (created)
            {
                ViewBag.Message = "User was created successfully.";
            }
            else
            {
                ViewBag.Message = "There was an error while creating the user.";
            }

            return View();
        }

        public ActionResult Update(int id)
        {
            if (HttpContext.Request.RequestType == "POST")
            {
                // Request is Post type; must be a submit
                var name = Request.Form["name"];

                // Get all of the clients
                var userss = Models.User.GetUsers();

                foreach (User usr in userss)
                {
                    // Find the client
                    if (usr.ID == id)
                    {
                        // Client found, now update his properties and save it.
                        usr.Name = name;
                        // Break through the loop
                        break;
                    }
                }

                // Update the clients in the disk
                System.IO.File.WriteAllText(Models.User.UserFile, JsonConvert.SerializeObject(userss));

                // Add the details to the View
                Response.Redirect("~/Client/Index?Message=Client_Updated");
            }


            // Create a model object.
            var user = new User();
            // Get the list of clients
            var users = Models.User.GetUsers();
            // Search within the clients
            foreach (User usr in users)
            {
                // If the client's ID matches
                if (usr.ID == id)
                {
                    user = usr;
                    // No need to further run the loop 
                    break;
                }
            }
            if (user == null)
            {
                // No client was found
                ViewBag.Message = "No client was found.";
            }
            return View(user);
        }

        public ActionResult Delete(int id)
        {
            // Get the clients
            var Users = Models.User.GetUsers();
            var deleted = false;
            // Delete the specific one.
            foreach (User usr in Users)
            {
                // Found the client
                if (usr.ID == id)
                {
                    // delete this client
                    var index = Users.IndexOf(usr);
                    Users.RemoveAt(index);

                    // Removed now save the data back.
                    System.IO.File.WriteAllText(Models.User.UserFile, JsonConvert.SerializeObject(Users));
                    deleted = true;
                    break;
                }
            }

            // Add the process details to the ViewBag
            if (deleted)
            {
                ViewBag.Message = "Client was deleted successfully.";
            }
            else
            {
                ViewBag.Message = "There was an error while deleting the client.";
            }
            return View();
        }
    }
}