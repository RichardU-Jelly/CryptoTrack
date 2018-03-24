using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace CryptoTrack.Models
{
    public class User
    {

        public static string UserFile = HttpContext.Current.Server.MapPath("~/App_Data/Users.json");

        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public static List<User> GetUsers()
        {
            List<User> user_list = new List<User>();
            if (File.Exists(UserFile))
            {
                string content = File.ReadAllText(UserFile);
                user_list = JsonConvert.DeserializeObject<List<User>>(content);
                return user_list;
            }
            else
            {
                File.Create(UserFile).Close();
                File.WriteAllText(UserFile, "[]");
                GetUsers();
            }
            return user_list;
        }

    }
}