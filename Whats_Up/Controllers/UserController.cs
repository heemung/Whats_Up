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
        public ActionResult AddUser(User registeredUser)
        {

            DataContext ORM = new DataContext();
            if (ModelState.IsValid)
            {
                ORM.Users.Add(registeredUser);
                ORM.SaveChanges();
                
            }
            return View("/Home/Index");
        }
    }
}