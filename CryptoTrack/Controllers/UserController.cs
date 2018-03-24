﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CryptoTrack.Models;
using Newtonsoft.Json;

namespace CryptoTrack.Controllers
{
    public class UserController : Controller
    {
        // GET: User
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

                if (pwd1 != pwd2)
                {
                    ViewBag.Message = "Your passwords does not match!";
                    return View();
                }
                else
                {
                    var UserFile = Models.User.UserFile;
                    var UserData = System.IO.File.ReadAllText(UserFile);
                    List<User> UserList = new List<User>();
                    UserList = JsonConvert.DeserializeObject<List<User>>(UserData);

                    var id = 1;
                    if (UserList.Count == 0)
                    {
                        UserList = new List<User>();
                    }
                    else
                    {
                        User LastUsr = UserList.First();
                        id = LastUsr.ID++;
                    }


                    User user = new User()
                    {
                        ID = Convert.ToInt16(id),
                        Name = name,
                        Password = pwd1
                    };

                    UserList.Add(user);
                    System.IO.File.WriteAllText(UserFile, JsonConvert.SerializeObject(UserList));
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