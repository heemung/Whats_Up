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
        //HomeController hometoinput = new HomeController();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public void AddUser(User registeredUser)
        {
            //TO DO
            //1. add user to database if exists

            string Email = registeredUser.Email;
            HttpCookie userCookie;           //making same cookie reguardless
            if (Request.Cookies["Email"] == null) //still throwing null
            {
                userCookie = new HttpCookie("RegisteredUser");
                userCookie.Values.Add("Email", registeredUser.Email);
                userCookie.Values.Add("Address", registeredUser.AddressLine);
            }
            else
            {
                userCookie = Request.Cookies["RegisteredUser"];
                userCookie.Values.Add("Email", registeredUser.Email);
                userCookie.Values.Add("Address", registeredUser.AddressLine);
            }

            Response.Cookies.Add(userCookie); //save cookie?


            /*HttpCookie cookie = new HttpCookie("some_cookie_name");
            HttpContext.Response.Cookies.Remove("some_cookie_name");
            HttpContext.Response.SetCookie(cookie);*/ //save cookie?

            //return usersEmail;

        }
    }
}