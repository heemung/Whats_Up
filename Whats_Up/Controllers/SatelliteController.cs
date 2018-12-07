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
        //json of satellite data for each category in a array
        public JArray SatCoordinates;

        //ORM private to this class.
        private DataContext db = new DataContext();
        // GET: Satellite
        public ActionResult Index()
        {
            return View("../Shared/Error");
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

                int lastSatRecordDB = 0;
                lastSatRecordDB = db.SatelliteN2YOs.Max(x => x.ID);

                int ApiOverLimit = db.SatelliteN2YOs.Find(lastSatRecordDB).TransactionsCount;

                ApiOverLimit += satelliteCategoies.Length;

                if (ApiOverLimit < 1000)
                {
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
                            SatErrors("Could not get HTTP Reponse");                           
                        }

                    }

                    //sends jarray to database method
                    ToDatabase(jSpaceObjects);

                    // sets json of satellite data for each category in a array
                    SatCoordinates = jSpaceObjects;

                    ViewBag.TableSatData = jSpaceObjects;
                }
                else
                {
                    SatErrors("Over API limit. Please Try Again Later");
                }
            }
            else
            {
                SatErrors("Error: Need to select 1 category");
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
            List<CheckBoxes> boxItem = new List<CheckBoxes>()
            {
                new CheckBoxes(){Name = "BeidouNavigationSystem", CheckName="satelliteCategoies", Value = "35",IsCheck = false},
                new CheckBoxes(){Name = "Brightest", CheckName="satelliteCategoies", Value = "1",IsCheck = false},
                new CheckBoxes(){Name = "Celestis", CheckName="satelliteCategoies", Value = "45",IsCheck = false},
                new CheckBoxes(){Name = "CubeSats", CheckName="satelliteCategoies", Value = "32",IsCheck = false},
                new CheckBoxes(){Name = "DisasterMonitoring", CheckName="satelliteCategoies", Value = "8",IsCheck = false},
                new CheckBoxes(){Name = "EarthResources", CheckName="satelliteCategoies", Value = "6",IsCheck = false},
                new CheckBoxes(){Name = "Education", CheckName="satelliteCategoies", Value = "29",IsCheck = false},
                new CheckBoxes(){Name = "Engineering", CheckName="satelliteCategoies", Value = "28",IsCheck = false},
                new CheckBoxes(){Name = "Experimental", CheckName="satelliteCategoies", Value = "19",IsCheck = false},
                new CheckBoxes(){Name = "Geodetic", CheckName="satelliteCategoies", Value = "27",IsCheck = false},
                new CheckBoxes(){Name = "Geostationary", CheckName="satelliteCategoies", Value = "10",IsCheck = false},
                new CheckBoxes(){Name = "GlobalPositioningSystem(GPS)Constellation", CheckName="satelliteCategoies", Value = "50",IsCheck = false},
                new CheckBoxes(){Name = "GlobalPositioningSystem(GPS)Operational", CheckName="satelliteCategoies", Value = "20",IsCheck = false},
                new CheckBoxes(){Name = "Globalstar", CheckName="satelliteCategoies", Value = "17",IsCheck = false},
                new CheckBoxes(){Name = "GlonassOperational", CheckName="satelliteCategoies", Value = "21",IsCheck = false},
                new CheckBoxes(){Name = "GOES", CheckName="satelliteCategoies", Value = "5",IsCheck = false},
                new CheckBoxes(){Name = "Gonets", CheckName="satelliteCategoies", Value = "40",IsCheck = false},
                new CheckBoxes(){Name = "Gorizont", CheckName="satelliteCategoies", Value = "12",IsCheck = false},
                new CheckBoxes(){Name = "Intelsat", CheckName="satelliteCategoies", Value = "11",IsCheck = false},
                new CheckBoxes(){Name = "Iridium", CheckName="satelliteCategoies", Value = "15",IsCheck = false},
                new CheckBoxes(){Name = "IRNSS", CheckName="satelliteCategoies", Value = "46",IsCheck = false},

            };
            //current email user???
            User currentFavUser = new User();
            List<Favorite> favoriteList = new List<Favorite>();

            currentFavUser.Email = "clayton.cox@gmail.com";

            IQueryable<Favorite> favQuery = db.Favorites.AsQueryable();
            favQuery = favQuery.Where(x => x.Email == currentFavUser.Email);
            favoriteList = favQuery.ToList();

            List<CheckBoxes> favBox = new List<CheckBoxes>();
            foreach (CheckBoxes box in boxItem)
            {
                    foreach (Favorite fav in favoriteList)
                    {
                        if (fav.Category == box.Name)
                        {
                            favBox.Add(new CheckBoxes() { IsCheck = true, CheckName = box.CheckName, Name = box.Name, Value = box.Value });
                        }
                        else if (!favBox.Any(x => x.Name == box.Name))
                        {
                            favBox.Add(new CheckBoxes() { IsCheck = box.IsCheck, CheckName = box.CheckName, Name = box.Name, Value = box.Value });
                        }
                    }


            }
            return favBox;
        }

        public ActionResult SatErrors(string error)
        {
            ViewBag.WhereError = error;
            return View("Error");
        }
    }
    
}
/*
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

            };
            return satCatDic;
        }*/

