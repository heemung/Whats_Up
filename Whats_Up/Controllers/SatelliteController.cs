using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Web.Configuration;

namespace Whats_Up.Controllers
{
    public class SatelliteController : Controller
    {
        // GET: Satellite
        public ActionResult Index()
        {

            string N2YO = WebConfigurationManager.AppSettings["N2YO"];

            UriBuilder builder = new UriBuilder
            {
                Scheme = "https",
                Host = "n2yo.com",
                Path = "/rest/v1/satellite/positions/25544//1/1/0/2/&apiKey=" + N2YO,
            };

            HttpWebRequest requestN2YO = WebRequest.CreateHttp(builder.ToString());
            requestN2YO.UserAgent = "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 64.0) Gecko / 20100101 Firefox / 64.0";

            HttpWebResponse reponse = (HttpWebResponse)requestN2YO.GetResponse();

            if(reponse.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(reponse.GetResponseStream());

                string output = reader.ReadToEnd();
                JObject jSpaceObject = JObject.Parse(output);

                ViewBag.ThisTest = jSpaceObject;
                ViewBag.ThisTest2 = builder.ToString();
            }

            return View();
        }

        //getting user selections for satellites categories 
        public List<string> GetSatCat()
        {
            



        }

        public Dictionary<string,int> AddingCatsToList()
        {
            Dictionary<string,int> satCatDic = new Dictionary<string, int>();

            satCatDic.Add("Yaogan", 36);
            satCatDic.Add("XMandSirius", 33);
            satCatDic.Add("WestfordNeedles", 37);
            satCatDic.Add("Weather", 3);
            satCatDic.Add("TV", 34);
            satCatDic.Add("Tsiklon", 41);
            satCatDic.Add("Tsikada", 42);
            satCatDic.Add("Tselina" ,44);
            satCatDic.Add("TrackingandDataRelaySatelliteSystem", 9);
            satCatDic.Add("Strela", 39);
            satCatDic.Add("Space&EarthScience", 26);
            satCatDic.Add("Search&Rescue", 7);
            satCatDic.Add("Satellite-BasedAugmentationSystem", 23);
            satCatDic.Add("RussianLEONavigation", 25);
            satCatDic.Add("Raduga", 13);
            satCatDic.Add("RadarCalibration", 18);
            satCatDic.Add("QZSS", 47);
            satCatDic.Add("Parus", 38);
            satCatDic.Add("Orbcomm", 16);
            satCatDic.Add("O3BNetworks", 43);
            satCatDic.Add("NOAA", 4);
            satCatDic.Add("NavyNavigationSatelliteSystem", 24);
            satCatDic.Add("Molniya", 14);
            satCatDic.Add("Military", 30);
            satCatDic.Add("Lemur", 49);
            satCatDic.Add("ISS", 2);
            satCatDic.Add("IRNSS", 46);
            satCatDic.Add("Iridium", 15);
            satCatDic.Add("Intelsat", 11);
            satCatDic.Add("Gorizont", 12);
            satCatDic.Add("Gonets", 40);
            satCatDic.Add("GOES", 5);
            satCatDic.Add("GlonassOperational", 21);
            satCatDic.Add("Globalstar", 17);
            satCatDic.Add("GlobalPositioningSystem(GPS)Operational", 20);
            satCatDic.Add("GlobalPositioningSystem(GPS)Constellation", 50);
            satCatDic.Add("Geostationary", 10);
            satCatDic.Add("Geodetic", 27);
            satCatDic.Add("Galileo", 22);
            satCatDic.Add("Flock", 48);
            satCatDic.Add("Experimental", 19);
            satCatDic.Add("Engineering", 28);
            satCatDic.Add("Education", 29);
            satCatDic.Add("EarthResources", 6);
            satCatDic.Add("DisasterMonitoring", 8);
            satCatDic.Add("CubeSats", 32);
            satCatDic.Add("Celestis", 45);
            satCatDic.Add("Brightest", 1);
            satCatDic.Add("BeidouNavigationSystem", 35);

            return satCatDic;
        }

        public ActionResult SatTracker()
        {

            string SatTracker = WebConfigurationManager.AppSettings["Space-Track"];

            UriBuilder builder = new UriBuilder
            {
                Scheme = "https",
                Host = "n2yo.com",
                Path = "/rest/v1/satellite/positions/25544//1/1/0/2/&apiKey=" + SatTracker,
            };

            HttpWebRequest requestN2YO = WebRequest.CreateHttp(builder.ToString());
            requestN2YO.UserAgent = "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 64.0) Gecko / 20100101 Firefox / 64.0";

            HttpWebResponse reponse = (HttpWebResponse)requestN2YO.GetResponse();

            if (reponse.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(reponse.GetResponseStream());

                string output = reader.ReadToEnd();
                JObject jSpaceObject = JObject.Parse(output);

                ViewBag.ThisTest = jSpaceObject;
                ViewBag.ThisTest2 = builder.ToString();
            }

            return View();
        }
    }
}