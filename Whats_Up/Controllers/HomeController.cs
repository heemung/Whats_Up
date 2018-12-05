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
        public JObject geoLocation;
        public string BingKey = WebConfigurationManager.AppSettings["Mapper"];
        public string GoogleKey = WebConfigurationManager.AppSettings["GMapper"];
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WhatsUp()
        {

            ViewBag.Message = "Your application description page.";
            ViewBag.GoogleKey = GoogleKey;

            return View();
        }
        public ActionResult InputLocation(User address)
        {
            string URL = "http://dev.virtualearth.net/REST/v1/Locations/US/adminDistrict/postalCode/locality/addressLine?" +
                "addressLine="+address.addressLine+"&postalCode="+address.postalCode+"&maxResults=5&key="+BingKey;
            HttpWebRequest request = WebRequest.CreateHttp(URL);
            request.UserAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64; rv: 47.0) Gecko / 20100101 Firefox / 47.0";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string output = reader.ReadToEnd();
                JObject JParser = JObject.Parse(output);

                geoLocation = JParser;

            }

            ViewBag.Testing = geoLocation;
            SatelliteController start = new SatelliteController();
            start.GetSatCat();
            ViewBag.Coordinates = start.SatCoordinates;

            return View("WhatsUp");
        }

        public ActionResult Creators()
        {
            ViewBag.Message = "Your 2018 C# .NET Developers";

            return View();
        }
    }
}