using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Web.Configuration;
using Whats_Up.Models;
using System.Data.Entity;

namespace Whats_Up.Controllers
{
    public class UserController : Controller
    {
        
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public void AddUser(User registeredUser)
        {

            if (registeredUser.Email == null)                       //checks if the user address is null and assigns dummy data if it is.
            {
                registeredUser.AddressLine = "no address";
                registeredUser.Email = "abcdefg@abcdefg.com";
            }

            WhatsUpDBEntities db = new WhatsUpDBEntities();

                if (db.Users.Find(registeredUser.Email) == null)
                {

                    db.Entry(registeredUser).State = EntityState.Added;
                    db.SaveChanges();                     
                }
        }
    }
}