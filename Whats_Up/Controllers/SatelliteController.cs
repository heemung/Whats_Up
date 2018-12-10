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
        public JArray SatCoordinates2;

        //ORM private to this class.
        private DataContext db = new DataContext();
        // GET: Satellite
        public ActionResult Index()
        {
            return View("../Shared/Error");
        }

        //getting user selections for satellites categories 
        public void GetSatCat(string[] satelliteCategoies, List<JObject> geoLocations)           
        {
            List<JArray> satsForEachAddress = new List<JArray>();

            string latitude; //42.327501
            string longitude; //"-83.048981"

            //api key
            string N2YO = WebConfigurationManager.AppSettings["N2YO"];

            //checking to make sure there is one category selected
            if (satelliteCategoies != null && geoLocations != null)
            {
                int lastSatRecordDB = 0;
                lastSatRecordDB = db.SatelliteN2YOs.Max(x => x.ID);                 //getting last entry in database

                int ApiOverLimit = db.SatelliteN2YOs.Find(lastSatRecordDB).TransactionsCount;
                ApiOverLimit += satelliteCategoies.Length;                          //getting transaction count from last entry and adding it to current transaction
                                        //TO DO
                if (ApiOverLimit < 1000)//NEED TO ACCOUNT FOR DOUBLE ADDRESS                                            //checking to see if API current Transaction is over 1000
                {
                    foreach (JObject location in geoLocations)
                    {
                        JArray jSpaceObjects = new JArray();                        //new jarray for each location object

                        latitude = location["results"][0]["geometry"]["location"]   //setting lat and long in each address to string
                            ["lat"].Value<string>();
                        longitude = location["results"][0]["geometry"]["location"]
                            ["lng"].Value<string>();

                        foreach (string categoryNum in satelliteCategoies)          //each add will have one request puts that request into a jobject and goes into an array
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

                                jSpaceObjects.Add(temp);                            //adding jobjects to jarray
                            }
                            else
                            {
                                //SatErrors("Could not get HTTP Reponse");
                            }

                        }
                        satsForEachAddress.Add(jSpaceObjects);


                        //sends jarray to database method
                        ToDatabase(jSpaceObjects);

                        // sets json of satellite data for each category in a array
                        SatCoordinates = jSpaceObjects;
                    }
                }
                else
                {
                    //SatErrors("Over API limit. Please Try Again Later");
                }
            }
            else
            {
                //SatInfo("Need to at least select 1 category and location");
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
        
        public void BigCategories()
        {

            List<CheckBoxes> AllItems = AddingCatsToList();

            List<string> Astronomy = new List<string>();
            Astronomy.Add(AllItems[0].Name);
            Astronomy.Add(AllItems[1].Name);
            Astronomy.Add(AllItems[2].Name);
            Astronomy.Add(AllItems[3].Name);
            Astronomy.Add(AllItems[4].Name);

            List<string> AtmosphericStudies = new List<string>();
            AtmosphericStudies.Add(AllItems[5].Name);
            AtmosphericStudies.Add(AllItems[6].Name);
            AtmosphericStudies.Add(AllItems[7].Name);
            AtmosphericStudies.Add(AllItems[8].Name);
            AtmosphericStudies.Add(AllItems[9].Name);
            AtmosphericStudies.Add(AllItems[10].Name);

            List<string> Communications = new List<string>();
            Communications.Add(AllItems[11].Name);
            Communications.Add(AllItems[12].Name);
            Communications.Add(AllItems[13].Name);
            Communications.Add(AllItems[14].Name);
            Communications.Add(AllItems[15].Name);

            List<string> Navigation = new List<string>();
            Navigation.Add(AllItems[16].Name);
            Navigation.Add(AllItems[17].Name);
            Navigation.Add(AllItems[18].Name);
            Navigation.Add(AllItems[19].Name);
            Navigation.Add(AllItems[20].Name);

            List<string> Reconaissance = new List<string>();
            Reconaissance.Add(AllItems[21].Name);
            Reconaissance.Add(AllItems[22].Name);
            Reconaissance.Add(AllItems[23].Name);
            Reconaissance.Add(AllItems[24].Name);
            Reconaissance.Add(AllItems[25].Name);

            List<string> RemoteSensing = new List<string>();
            RemoteSensing.Add(AllItems[26].Name);
            RemoteSensing.Add(AllItems[27].Name);
            RemoteSensing.Add(AllItems[23].Name);
            RemoteSensing.Add(AllItems[24].Name);
            RemoteSensing.Add(AllItems[25].Name);

            List<string> SearchRescue = new List<string>();
            SearchRescue.Add(AllItems[21].Name);
            SearchRescue.Add(AllItems[22].Name);
            SearchRescue.Add(AllItems[23].Name);
            SearchRescue.Add(AllItems[24].Name);
            SearchRescue.Add(AllItems[25].Name);

            List<string> SpaceExploration = new List<string>();
            SpaceExploration.Add(AllItems[21].Name);
            SpaceExploration.Add(AllItems[22].Name);
            SpaceExploration.Add(AllItems[23].Name);
            SpaceExploration.Add(AllItems[24].Name);
            SpaceExploration.Add(AllItems[25].Name);

            List<string> Weather = new List<string>();
            Weather.Add(AllItems[21].Name);
            Weather.Add(AllItems[22].Name);
            Weather.Add(AllItems[23].Name);
            Weather.Add(AllItems[24].Name);
            Weather.Add(AllItems[25].Name);

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
                new CheckBoxes(){Name = "ISS", CheckName="satelliteCategoies", Value = "2",IsCheck = false},
                new CheckBoxes(){Name = "Lemur", CheckName="satelliteCategoies", Value = "49",IsCheck = false},
                new CheckBoxes(){Name = "Military", CheckName="satelliteCategoies", Value = "30",IsCheck = false},
                new CheckBoxes(){Name = "Molniya", CheckName="satelliteCategoies", Value = "14",IsCheck = false},
                new CheckBoxes(){Name = "NavyNavigationSatelliteSystem", CheckName="satelliteCategoies", Value = "46",IsCheck = false},
                new CheckBoxes(){Name = "NOAA", CheckName="satelliteCategoies", Value = "4",IsCheck = false},
                new CheckBoxes(){Name = "O3BNetworks", CheckName="satelliteCategoies", Value = "43",IsCheck = false},
                new CheckBoxes(){Name = "Orbcomm", CheckName="satelliteCategoies", Value = "16",IsCheck = false},
                new CheckBoxes(){Name = "Parus", CheckName="satelliteCategoies", Value = "38",IsCheck = false},
                new CheckBoxes(){Name = "QZSS", CheckName="satelliteCategoies", Value = "47",IsCheck = false},
                new CheckBoxes(){Name = "RadarCalibration", CheckName="satelliteCategoies", Value = "18",IsCheck = false},
                new CheckBoxes(){Name = "Raduga", CheckName="satelliteCategoies", Value = "13",IsCheck = false},
                new CheckBoxes(){Name = "RussianLEONavigation", CheckName="satelliteCategoies", Value = "25",IsCheck = false},
                new CheckBoxes(){Name = "Satellite-BasedAugmentationSystem", CheckName="satelliteCategoies", Value = "23",IsCheck = false},
                new CheckBoxes(){Name = "Search&Rescue", CheckName="satelliteCategoies", Value = "7",IsCheck = false},
                new CheckBoxes(){Name = "Space&EarthScience", CheckName="satelliteCategoies", Value = "26",IsCheck = false},
                new CheckBoxes(){Name = "Strela", CheckName="satelliteCategoies", Value = "39",IsCheck = false},
                new CheckBoxes(){Name = "TrackingandDataRelaySatelliteSystem", CheckName="satelliteCategoies", Value = "9",IsCheck = false},
                new CheckBoxes(){Name = "Tselina", CheckName="satelliteCategoies", Value = "44",IsCheck = false},
                new CheckBoxes(){Name = "Tsikada", CheckName="satelliteCategoies", Value = "42",IsCheck = false},
                new CheckBoxes(){Name = "Tsiklon", CheckName="satelliteCategoies", Value = "41",IsCheck = false},
                new CheckBoxes(){Name = "TV", CheckName="satelliteCategoies", Value = "34",IsCheck = false},
                new CheckBoxes(){Name = "Weather", CheckName="satelliteCategoies", Value = "3",IsCheck = false},
                new CheckBoxes(){Name = "WestfordNeedles", CheckName="satelliteCategoies", Value = "37",IsCheck = false},
                new CheckBoxes(){Name = "XMandSirius", CheckName="satelliteCategoies", Value = "33",IsCheck = false},
                new CheckBoxes(){Name = "Yaogan", CheckName="satelliteCategoies", Value = "36",IsCheck = false},


            };
            return boxItem;
        }

        public List<CheckBoxes> SettingCheckBoxes(string currentUserEmail, List<string> bigCategory)
        {
            List<Favorite> favoriteList = new List<Favorite>();
            if (currentUserEmail == null)                            //first time log in or no log in
            {

                if("cookie there" == "cookie there")
                {
                    //COOKIE CHECK CAN GO HERE
                    currentUserEmail = "test1234@gmail.com";
                }
                else
                {
                    currentUserEmail = "1";                         //meaning no favorite check, user did not select to sign in
                }
            }

            IQueryable<Favorite> favQuery = db.Favorites.AsQueryable();     //favorites search in DB
            favQuery = favQuery.Where(x => x.Email == currentUserEmail);


            favoriteList = favQuery.ToList();                               //to List and are there favorites?
            if (favoriteList.Count != 0)
            {
                //add big list check here
                List<CheckBoxes> favBox = new List<CheckBoxes>();
                foreach (CheckBoxes box in AddingCatsToList())
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
            else
            {
                return AddingCatsToList();
            }
        }

        public ActionResult SaveFavorties(Favorite newFavorite)
        {
            DataContext db = new DataContext();
            db.Favorites.Add(newFavorite);
            db.SaveChanges();
            return RedirectToAction("Favorites"); 
        }

        public ActionResult SatErrors(string error)
        {
            ViewBag.ErrorBag = error;
            return View("Error");
        }

        public ActionResult SatInfo(string info)
        {
            ViewBag.InfoBag = info;
            return View("WhatsUp");
        }
    }
    
}

/*
 *         //testing small sample
        public Dictionary<string, int> AddingCatsToList1()
        {
            Dictionary<string, int> satCatDic = new Dictionary<string, int>
            {
                { "Yaogan", 36 },
                { "XMandSirius", 33 },
                { "WestfordNeedles", 37 },
            };

            return satCatDic;
        }*/

