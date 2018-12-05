using Newtonsoft.Json.Linq;
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
        public static string geoLat;
        public static string geoLong;
        public string BingKey = WebConfigurationManager.AppSettings["Mapper"];
        public string GoogleKey = WebConfigurationManager.AppSettings["GMapper"];
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WhatsUp()
        {

            ViewBag.GoogleKey = GoogleKey;
            return View();
        }
        public ActionResult InputLocation(User address)
        {
            JObject JParser = new JObject();

            string URL = "https://maps.googleapis.com/maps/api/geocode/json?address="+address.addressLine+"&key=" + GoogleKey;
            HttpWebRequest request = WebRequest.CreateHttp(URL);
            request.UserAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64; rv: 47.0) Gecko / 20100101 Firefox / 47.0";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string output = reader.ReadToEnd();
                JParser = JObject.Parse(output);
            }

            geoLat = JParser["results"][0]["geometry"]["location"]["lat"].Value<string>();
            geoLong = JParser["results"][0]["geometry"]["location"]["lng"].Value<string>();
            //geoLat = JParser["resourceSets"][0]["resources"][0]["geocodePoints"][0]["coordinates"][0].Value<string>();
            //geoLong = JParser["resourceSets"][0]["resources"][0]["geocodePoints"][0]["coordinates"][1].Value<string>();
            SatelliteController start = new SatelliteController();
            start.GetSatCat();
            ViewBag.GoogleLat = double.Parse(geoLat);
            ViewBag.GoogleLong = double.Parse(geoLong);
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