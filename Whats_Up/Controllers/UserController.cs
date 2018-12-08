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
        HomeController hometoinput = new HomeController();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }
             public ActionResult AddUser(User registeredUser)
             {
             HttpCookie userCookie;
                if (Request.Cookies["Email"] == null)
                {
                userCookie = new HttpCookie("Email", registeredUser.Email);
                }
             else
                {
                userCookie = Request.Cookies["Email"];
                }
            userCookie.Value = registeredUser.Email;
            Response.Cookies.Add(userCookie);


             string usersEmail;
             if (Request.Cookies["Email"] != null)
             {
           
                usersEmail = Request.Cookies["Email"].Value;
                ViewBag.EmailTest = usersEmail;
             }




            DataContext ORM = new DataContext();
                List<User> userList = ORM.Users.ToList<User>();
                foreach(var x in userList)
             {
                if(registeredUser.Email == x.Email)
                {
                    List<Favorite> userFavorite = new List<Favorite>();
                    foreach (var y in ORM.Favorites.Where(y => y.Email == registeredUser.Email))
                    {
                        userFavorite.Add(y);
                    }
                    ViewBag.Favorites = userFavorite;
                        return View("../Home/WhatsUp");
                }
             }
                 if (ModelState.IsValid)
                 {
                    ORM.Entry(registeredUser).State = EntityState.Added;
                    ORM.SaveChanges();

                 }
                 
                 return View("../Home/WhatsUp");
                
             } 
    }
}