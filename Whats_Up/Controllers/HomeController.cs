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
        public SatelliteController checkbox = new SatelliteController();
        public string GeoKey = WebConfigurationManager.AppSettings["GMapper"];
        public string GoogleKey = WebConfigurationManager.AppSettings["GMapper"];

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WhatsUp()
        {
            ViewBag.GoogleKey = GoogleKey;

            TempData["SatList"] = checkbox.AddingCatsToList();
            return View();
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
        }


        public ActionResult InputLocation(User address, string[] satelliteCategoies)
        {
            //string addresses for testing
            string address1 ="indianapolis", address2 = "south korea";

            bool addressesIs2 = false;
            List<string> addresses = new List<string>();
            List<string> geoLocations = new List<string>();

            if (address2 != null)
            {

                string formattedAddress1 = AddressForGoogle(address1);
                string formattedAddress2 = AddressForGoogle(address2);

                addresses.Add(formattedAddress1);
                addresses.Add(formattedAddress2);
                addressesIs2 = true;
            }
            else
            {
                string formattedAddress1 = AddressForGoogle(address1);
                addresses.Add(formattedAddress1);
                addressesIs2 = false;
            }

            foreach (string listFormattedAddress in addresses)
            {
                JObject JParser = new JObject();

                string URL = "https://maps.googleapis.com/maps/api/geocode/json?address=" + listFormattedAddress + "&key=" + GeoKey;
                HttpWebRequest request = WebRequest.CreateHttp(URL);
                request.UserAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64; rv: 47.0) Gecko / 20100101 Firefox / 47.0";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string output = reader.ReadToEnd();
                    JParser = JObject.Parse(output);

                }

                geoLocations.Add(geoLat = JParser["results"][0]["geometry"]["location"]["lat"].Value<string>());
                geoLocations.Add(geoLong = JParser["results"][0]["geometry"]["location"]["lng"].Value<string>());
            }

            SatelliteController start = new SatelliteController();
            start.GetSatCat(satelliteCategoies);

            if(addressesIs2 == false)
            {
                geoLocations[0] = ViewBag.GoogleLat = double.Parse(geoLat);
                geoLocations[1] = ViewBag.GoogleLong = double.Parse(geoLong);
            }
            else
            {
                geoLocations[0] = ViewBag.GoogleLat = double.Parse(geoLat);
                geoLocations[1] = ViewBag.GoogleLong = double.Parse(geoLong);
                geoLocations[3] = ViewBag.GoogleLat2 = double.Parse(geoLat);
                geoLocations[4] = ViewBag.GoogleLong2 = double.Parse(geoLong);
            }

            TempData["SatList"] = checkbox.AddingCatsToList();
            ViewBag.Coordinates = start.SatCoordinates;
            ViewBag.Coordinates2 = start.SatCoordinates;
            return View("WhatsUp");
        }

        public ActionResult Creators()
        {
            ViewBag.Message = "Your 2018 C# .NET Developers";

            return View();
        }
    }
}