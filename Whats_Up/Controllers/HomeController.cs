﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using Whats_Up.Models;

namespace Whats_Up.Controllers
{
    public class HomeController : Controller
    {

        public SatelliteController satController = new SatelliteController();
        public UserController currentUserController = new UserController();

        private List<CheckBoxes> homeCheckBoxData = new List<CheckBoxes>();
        private List<CheckBoxes> homeSatData = new List<CheckBoxes>();
        private List<JObject> resultFromLocation = new List<JObject>();



        public string GeoKey = WebConfigurationManager.AppSettings["GMapper"];
        public string GoogleKey = WebConfigurationManager.AppSettings["GMapper"];

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WhatsUp()
        {

            ViewBag.GoogleKey = GoogleKey;                                  //sends google key for embedded map

            return View();
        }

        public ActionResult FormCollection(User currentUser, string[] satelliteCategoies, string address2)
        {
            //TEST BIG CAT SELECTION
            List<string> bigCategory = new List<string>();
            //TEST END


            resultFromLocation = InputLocation(currentUser.AddressLine, address2);     //calling method to get user information in home controller

            satController.GetSatCat(satelliteCategoies, resultFromLocation);                //calling satcat and passing geo and user view selected categories, this posts data into two public jarrays in sat controller
            homeCheckBoxData = satController.SettingCheckBoxes(currentUser.Email, bigCategory);                //calling setting the checkboxes. this makes these into a gobal private list here

            if (homeCheckBoxData.Count == 0)
            {
                TempData["SatList"] = satController.AddingCatsToList();
            }
            else
            {
                TempData["SatList"] = homeCheckBoxData;
            }

            ViewBag.Coordinates = satController.SatCoordinates;
            ViewBag.Coordinates2 = satController.SatCoordinates;

            if (resultFromLocation.Count == 1)
            {
                string latitude = resultFromLocation[0]["results"][0]
                    ["geometry"]["location"]["lat"].Value<string>();   //setting lat and long in each address to string["lat"].Value<string>();
                string longitude = resultFromLocation[0]["results"][0]
                    ["geometry"]["location"]["lng"].Value<string>();
                ViewBag.GoogleLat = double.Parse(latitude);
                ViewBag.GoogleLong = double.Parse(longitude);
            }
            else if (resultFromLocation.Count == 2)
            {
                string latitude1 = resultFromLocation[0]["results"][0]
                    ["geometry"]["location"]["lat"].Value<string>();   //setting lat and long in each address to string["lat"].Value<string>();
                string longitude1 = resultFromLocation[0]["results"][0]
                    ["geometry"]["location"]["lng"].Value<string>();
                string latitude2 = resultFromLocation[1]["results"][0]
                    ["geometry"]["location"]["lat"].Value<string>();   //setting lat and long in each address to string["lat"].Value<string>();
                string longitude2 = resultFromLocation[1]["results"][0]
                    ["geometry"]["location"]["lng"].Value<string>();
                ViewBag.GoogleLat = double.Parse(latitude1);
                ViewBag.GoogleLong = double.Parse(longitude1);
                ViewBag.GoogleLat2 = double.Parse(latitude2);
                ViewBag.GoogleLong2 = double.Parse(longitude2);
            }

            return View("WhatsUp");
        }

        public string AddressForGoogle(string address)
        {
            if (address.Contains(' '))
            {
                string addressForLink = address;
                char[] tempForLoop = new char[addressForLink.Length];

                for (int i = 0; i < addressForLink.Length; i++)
                {
                    if (addressForLink[i] == ' ')
                    {
                        tempForLoop[i] = '+';
                    }
                    else
                    {
                        tempForLoop[i] = addressForLink[i];
                    }

                }
                string temp = new string(tempForLoop);
                return temp;
            }
            else
            {
                return address;
            }
        }                   //formats address spaces to +


        public List<JObject> InputLocation(string address1, string address2)  //takes two address
        {

            List<string> addresses = new List<string>();                    //making list to store 1 or 2 addresses
            List<JObject> geoLocations = new List<JObject>();                 //makeing list to store first address lat-long and second lat long in jobject

            if (address2 != null)
            {

                string formattedAddress1 = AddressForGoogle(address1);      //method use to format " " to +
                string formattedAddress2 = AddressForGoogle(address2);

                addresses.Add(formattedAddress1);
                addresses.Add(formattedAddress2);

            }
            else
            {                                                               //used for only 1 address
                string formattedAddress1 = AddressForGoogle(address1);
                addresses.Add(formattedAddress1);
            }

            foreach (string listFormattedAddress in addresses)              // will make google api requests for each address
            {
                JObject JParser = new JObject();

                string URL = "https://maps.googleapis.com/maps/api/geocode/json?address=" + listFormattedAddress + 
                    "&key=" + GeoKey;
                HttpWebRequest request = WebRequest.CreateHttp(URL);

                request.UserAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64; rv: 47.0) " +
                    "Gecko / 20100101 Firefox / 47.0";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)               //if code 200 else will go to error page
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string output = reader.ReadToEnd();
                    JParser = JObject.Parse(output);

                }
                                                                            
                geoLocations.Add(JParser);                                  //adds jobject to list
            }

            return geoLocations;

        }

        public ActionResult Creators()
        {
            ViewBag.Message = "Your 2018 C# .NET Developers";

            return View();
        }

        public User GetCookie() //getting the cookie to a new user object.
        {
            User cookieUser = new User();
            if(HttpContext.Request.Cookies["RegisteredUser"] != null)
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("RegisteredUser");
                cookieUser.Email = cookie.Values["Email"].ToString();
                cookieUser.AddressLine = cookie.Values["Address"].ToString();
            }

            return cookieUser;
        }
    }
}

