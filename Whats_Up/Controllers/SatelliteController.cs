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
    public class SatelliteController : Controller
    {
        public static JArray SatCoordinates;

        private DataContext db = new DataContext();
        // GET: Satellite
        public ActionResult Index()
        {
            return View();
        }

        //getting user selections for satellites categories 
        public void GetSatCat()
        {
            string latitude = "42.327501";
            string longitude = "-83.048981";
            //HomeController home = new HomeController();
            //latitude = HomeController.geoLocation["resourceSets"]["resources"]["geocodePoints"]["coordinates"][0].Value<string>();
            //longitude = HomeController.geoLocation["resourceSets"]["resources"]["geocodePoints"]["coordinates"][1].Value<string>();


            /////////////////goes in method
            List<string> userListSelection = new List<string>();
            /////////////////
            //api key
            string N2YO = WebConfigurationManager.AppSettings["N2YO"];


            Dictionary<string, int> Test = AddingCatsToList();

            //TEMP TESTING ONLY START
            foreach (KeyValuePair<string, int> k in Test)
            {
                userListSelection.Add(k.Key);
            }
            //TEMP END TEST

            int arrayCatCount = userListSelection.Count;
            int[] userCatInt = new int[arrayCatCount];

            //checking to make sure there is one category selected
            if (Test.Count != 0)
            {
                //could be simplified
                int tempForArray = 0;
                foreach (string s in userListSelection)
                {
                    foreach (KeyValuePair<string, int> satCat in Test)
                    {
                        if (s == satCat.Key)
                        {
                            userCatInt[tempForArray] = satCat.Value;
                            tempForArray++;
                        }
                    }

                }
                JArray jSpaceObjects = new JArray();
                foreach (int catNum in userCatInt)
                {
                    UriBuilder builder = new UriBuilder
                    {
                        Scheme = "https",
                        Host = "n2yo.com",
                        Path = "rest/v1/satellite/above/" + latitude + "/" + longitude + "/0/70/" + catNum + "/&apiKey=" + N2YO,
                    };

                    HttpWebRequest requestN2YO = WebRequest.CreateHttp(builder.ToString());
                    requestN2YO.UserAgent = "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 64.0) Gecko / 20100101 Firefox / 64.0";

                    HttpWebResponse reponse = (HttpWebResponse)requestN2YO.GetResponse();

                    if (reponse.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader reader = new StreamReader(reponse.GetResponseStream());

                        string output = reader.ReadToEnd();

                        JObject temp = JObject.Parse(output);
                        jSpaceObjects.Add(temp);
                    }
                    else
                    {
                        ViewBag.Error = "Could not get HTTP Reponse";
                        //return View("/Shared/Error");
                    }

                }

                //sends jarray to database method
                ToDatabase(jSpaceObjects);
                ViewBag.TableSatData = jSpaceObjects;
                SatCoordinates = jSpaceObjects;
                //TempData["DataToBase"] = jSpaceObjects;
                //return View();
                //return View("Index");
            }
            else
            {
                ViewBag.SamePageError = "Error. Need to select 1 category";
                //return View();
            }
        }
        //taking the Jarray to push to DB
        public void ToDatabase(JArray satdata)
        {
            string atTime = DateTime.Now.ToString();

            // creating a new db object
            SatelliteN2YO dbObj = new SatelliteN2YO();

            //iterating through the satdata table returnong the request
            foreach (var SatRequest in satdata)
            {
                //returning the data in correct format
                dbObj.Category = SatRequest["info"]["category"].Value<string>();
                dbObj.TransactionsCount = SatRequest["info"]["transactionscount"].Value<int>();
                dbObj.SatCount = SatRequest["info"]["satcount"].Value<int>();

                if (SatRequest["info"]["satcount"].Value<int>() != 0)
                {
                    foreach (var satData in SatRequest["above"])
                    {

                        //getting all these values from the model
                        dbObj.SatId = satData["satid"].Value<int>();
                        dbObj.SatName = satData["satname"].Value<string>();
                        dbObj.Designator = satData["intDesignator"].Value<string>();
                        dbObj.LaunchDate = satData["launchDate"].Value<string>();
                        dbObj.SatLat = satData["satlat"].Value<double>();
                        dbObj.SatLng = satData["satlng"].Value<double>();
                        dbObj.SatAlt = satData["satalt"].Value<double>();
                        dbObj.AtTime = atTime;
                        db.Entry(dbObj).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    //adding to the DB
                }
                else
                {
                    dbObj.SatId = null;
                    dbObj.SatName = null;
                    dbObj.Designator = null;
                    dbObj.LaunchDate = null;
                    dbObj.SatLat = null;
                    dbObj.SatLng = null;
                    dbObj.SatAlt = null;
                    dbObj.AtTime = null;

                    db.Entry(dbObj).State = EntityState.Added;
                    db.SaveChanges();
                }
            }
            //saving chnages actually made to the DB
        }

        //testing small sample
        public Dictionary<string, int> AddingCatsToList()
        {
            Dictionary<string, int> satCatDic = new Dictionary<string, int>
            {
                { "Yaogan", 36 },
                { "XMandSirius", 33 },
                { "WestfordNeedles", 37 },
            };

            return satCatDic;
        }

        //better to put in database?
       /*public Dictionary<string,int> AddingCatsToList()
        {
            Dictionary<string, int> satCatDic = new Dictionary<string, int>
            {
                { "Yaogan", 36 },
                { "XMandSirius", 33 },
                { "WestfordNeedles", 37 },
                { "Weather", 3 },
                { "TV", 34 },
                { "Tsiklon", 41 },
                { "Tsikada", 42 },
                { "Tselina", 44 },
                { "TrackingandDataRelaySatelliteSystem", 9 },
                { "Strela", 39 },
                { "Space&EarthScience", 26 },
                { "Search&Rescue", 7 },
                { "Satellite-BasedAugmentationSystem", 23 },
                { "RussianLEONavigation", 25 },
                { "Raduga", 13 },
                { "RadarCalibration", 18 },
                { "QZSS", 47 },
                { "Parus", 38 },
                { "Orbcomm", 16 },
                { "O3BNetworks", 43 },
                { "NOAA", 4 },
                { "NavyNavigationSatelliteSystem", 24 },
                { "Molniya", 14 },
                { "Military", 30 },
                { "Lemur", 49 },
                { "ISS", 2 },
                { "IRNSS", 46 },
                { "Iridium", 15 },
                { "Intelsat", 11 },
                { "Gorizont", 12 },
                { "Gonets", 40 },
                { "GOES", 5 },
                { "GlonassOperational", 21 },
                { "Globalstar", 17 },
                { "GlobalPositioningSystem(GPS)Operational", 20 },
                { "GlobalPositioningSystem(GPS)Constellation", 50 },
                { "Geostationary", 10 },
                { "Geodetic", 27 },
                { "Galileo", 22 },
                { "Flock", 48 },
                { "Experimental", 19 },
                { "Engineering", 28 },
                { "Education", 29 },
                { "EarthResources", 6 },
                { "DisasterMonitoring", 8 },
                { "CubeSats", 32 },
                { "Celestis", 45 },
                { "Brightest", 1 },
                { "BeidouNavigationSystem", 35 }
            };

            return satCatDic;
        }*/

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