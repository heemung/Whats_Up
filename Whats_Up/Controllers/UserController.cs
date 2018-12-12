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
            //TO DO
            //1. add user to database if exists
            
            DataContext db = new DataContext();

            if(db.Users.Find(registeredUser.Email) == null)
            {
                
                db.Entry(registeredUser).State = EntityState.Added;
                db.SaveChanges();                           //TO DO FAILS HERE!!!
            }         
            /*HttpCookie cookie = new HttpCookie("some_cookie_name");
            HttpContext.Response.Cookies.Remove("some_cookie_name");
            HttpContext.Response.SetCookie(cookie);*/ //save cookie?

            //return usersEmail;

        }
    }
}