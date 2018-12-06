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
        public JArray SatCoordinates;

        private DataContext db = new DataContext();
        // GET: Satellite
        public ActionResult Index()
        {
            return View();
        }

        //getting user selections for satellites categories 
        public void GetSatCat(string[] satelliteCategoies)
        {
            string latitude; //42.327501
            string longitude; //"-83.048981"
            latitude = HomeController.geoLat;
            longitude = HomeController.geoLong;

            //api key
            string N2YO = WebConfigurationManager.AppSettings["N2YO"];

            //checking to make sure there is one category selected
            if (satelliteCategoies.Length != 0)
            {

                JArray jSpaceObjects = new JArray();

                foreach (string categoryNum in satelliteCategoies)
                {
                    UriBuilder builder = new UriBuilder
                    {
                        Scheme = "https",
                        Host = "n2yo.com",
                        Path = "rest/v1/satellite/above/" + latitude + "/" + longitude + "/0/70/" + categoryNum + "/&apiKey=" + N2YO,
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

                //testing location
                ViewBag.TableSatData = jSpaceObjects;
                SatCoordinates = jSpaceObjects;
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
        public Dictionary<string, int> AddingCatsToList1()
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
       public List<CheckBoxes> AddingCatsToList()
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
            };

            List<CheckBoxes> boxItem = new List<CheckBoxes>()
            {

                new CheckBoxes(){Name = "BeidouNavigationSystem", CheckName="satelliteCategoies", Value = "35",IsCheck = false},
                new CheckBoxes(){Name = "Brightest", CheckName="satelliteCategoies", Value = "1",IsCheck = false},
                new CheckBoxes(){Name = "Celestis", CheckName="satelliteCategoies", Value = "45",IsCheck = false},
                new CheckBoxes(){Name = "CubeSats", CheckName="satelliteCategoies", Value = "32",IsCheck = false},
                new CheckBoxes(){Name = "DisasterMonitoring", CheckName="satelliteCategoies", Value = "8",IsCheck = false},
                new CheckBoxes(){Name = "EarthResources", CheckName="satelliteCategoies", Value = "6",IsCheck = false},
                new CheckBoxes(){Name = "Education", CheckName="satelliteCategoies", Value = "29",IsCheck = false},
                new CheckBoxes(){Name = "Engineering", CheckName="satelliteCategoies", Value = "28",IsCheck = true},
                new CheckBoxes(){Name = "Experimental", CheckName="satelliteCategoies", Value = "19",IsCheck = false},

            };

            return boxItem;
        }
    }
}