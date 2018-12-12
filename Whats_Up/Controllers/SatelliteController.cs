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
        public JArray SatCoordinates3;
        public JArray SatCoordinates1Address;
        public string Error;
        public string Info;
        public string TransCount;
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
            //reset messages
            Error = "";
            Info = "";
            TransCount = "";
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

                int transactionMulti = 1;
                if(geoLocations.Count == 2)                                         //if there is more than one address then the requests will run twice
                {
                    transactionMulti = 2;
                }

                if (ApiOverLimit < 1000*transactionMulti)                           //checking to see if API current Transaction is over 1000
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
                                Error = "Could not get HTTP Reponse";
                            }

                        }
                        satsForEachAddress.Add(jSpaceObjects);


                        //sends jarray to database method
                        ToDatabase(jSpaceObjects);

                        // sets json of satellite data for each category in a array

                    }
                    CompareSatData(satsForEachAddress);
                }
                else
                {
                    TransCount ="Over API limit. Please Try Again Later";
                }
            }
            else
            {
                Info = "Need to at least select 1 category and location";
            }
        }

        public void CompareSatData(List<JArray> returnFromSatCat)
        {
            bool oneAddress = true;
            JArray comparision1;
            JArray comparision2;
            JArray comparisionSame = new JArray();
            JArray comparision1Unque = new JArray();
            JArray comparision2Unque = new JArray();
            Dictionary<string, JObject> satAdress1 = new Dictionary<string, JObject>();
            Dictionary<string, JObject> satAdress2 = new Dictionary<string, JObject>();
            try
            {
                if (returnFromSatCat.Count == 1)
                {
                    oneAddress = true;
                    SatCoordinates1Address = returnFromSatCat[0];
                    //for testing
                    //SatCoordinates = returnFromSatCat[0];
                }
                else if (returnFromSatCat.Count == 2)
                {
                    oneAddress = false;
                    comparision1 = returnFromSatCat[0];
                    comparision2 = returnFromSatCat[1];


                    foreach (dynamic outerArray1 in comparision1)
                    {
                        if (outerArray1["info"]["satcount"] != 0)               //checking if there is satellites in the list
                        {
                            foreach(JObject innerArray1 in outerArray1["above"])
                            {
                                if (!satAdress1.ContainsKey(innerArray1["satid"].ToString()))
                                {
                                    satAdress1.Add(innerArray1["satid"].ToString(), innerArray1);
                                }
                            }
                        }
                    }
                    foreach (dynamic outerArray2 in comparision2)
                    {
                        if (outerArray2["info"]["satcount"] != 0)               //checking if there is satellites in the list
                        {
                            foreach (JObject innerArray2 in outerArray2["above"])
                            {
                                if (!satAdress2.ContainsKey(innerArray2["satid"].ToString()))
                                {
                                satAdress2.Add(innerArray2["satid"].ToString(), innerArray2);
                                }
                            }
                        }
                    }
                    
                if(satAdress1.Count == 0 || satAdress2.Count == 0)              //checking to make sure lists wont be comparing nulls
                {
                    if(satAdress2.Count == 0)
                    {
                            foreach (var j in satAdress1)
                            {
                                comparision1Unque.Add(j.Value);
                            }
                    }
                    else
                    {
                            foreach (var j in satAdress2)
                            {
                                comparision2Unque.Add(j.Value);
                            }
                    }
                }
                else
                {
                    foreach(string key in satAdress1.Keys)
                    {
                        if(satAdress2.Keys.Contains(key))
                        {
                            comparisionSame.Add(satAdress2[key]);
                            satAdress2.Remove(key);

                        }
                        else
                        {
                            comparision1Unque.Add(satAdress1[key]);
                        }
                    }
                    if(satAdress2.Count !=0)
                    {
                        foreach(var j in satAdress2)
                        {
                            comparision2Unque.Add(j.Value);
                        }
                    }

                }
            } //end else if
                if (oneAddress == false)
                {
                    SatCoordinates = comparisionSame;
                    SatCoordinates2 = comparision1Unque;
                    SatCoordinates3 = comparision2Unque;

                }

            } //end try
            catch (Exception)
            {

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
        
        public List<string> BigCategories(string bigListSelection)
        {

            List<CheckBoxes> AllItems = AddingCatsToList();

            switch(bigListSelection)
            {
                case "Astronomy":
                    List<string> Astronomy = new List<string>
                    {
                        AllItems[0].Name,
                        AllItems[1].Name,
                        AllItems[2].Name,
                        AllItems[3].Name,
                        AllItems[4].Name
                    };
                    return Astronomy;
                    
                case "AtmosphericStudies":
                    List<string> AtmosphericStudies = new List<string>
                    {
                        AllItems[5].Name,
                        AllItems[6].Name,
                        AllItems[7].Name,
                        AllItems[8].Name,
                        AllItems[9].Name,
                        AllItems[10].Name
                    };
                    return AtmosphericStudies;

                case "Communications":

                    List<string> Communications = new List<string>
                    {
                        AllItems[11].Name,
                        AllItems[12].Name,
                        AllItems[13].Name,
                        AllItems[14].Name,
                        AllItems[15].Name
                    };

                    return Communications;

                case "Navigation":
                    List<string> Navigation = new List<string>
                    {
                        AllItems[16].Name,
                        AllItems[17].Name,
                        AllItems[18].Name,
                        AllItems[19].Name,
                        AllItems[20].Name
                    };

                    return Navigation;

                case "Reconaissance":

                    List<string> Reconaissance = new List<string>
                    {
                        AllItems[21].Name,
                        AllItems[22].Name,
                        AllItems[23].Name,
                        AllItems[24].Name,
                        AllItems[25].Name
                    };

                    return Reconaissance;

                case "RemoteSensing":
                    List<string> RemoteSensing = new List<string>
                    {
                        AllItems[26].Name,
                        AllItems[27].Name,
                        AllItems[23].Name,
                        AllItems[24].Name,
                        AllItems[25].Name
                    };

                    return RemoteSensing;
                case "SearchRescue":

                    List<string> SearchRescue = new List<string>
                    {
                        AllItems[21].Name,
                        AllItems[22].Name,
                        AllItems[23].Name,
                        AllItems[24].Name,
                        AllItems[25].Name
                    };
                    return SearchRescue;

                case "SpaceExploration":
                    List<string> SpaceExploration = new List<string>
                    {
                        AllItems[21].Name,
                        AllItems[22].Name,
                        AllItems[23].Name,
                        AllItems[24].Name,
                        AllItems[25].Name
                    };

                    return SpaceExploration;

                case "Weather":


                    List<string> Weather = new List<string>
                    {
                        AllItems[21].Name,
                        AllItems[22].Name,
                        AllItems[23].Name,
                        AllItems[24].Name,
                        AllItems[25].Name
                    };
                    return Weather;

                default:
                     List<string> temp = new List<string>();

                    return temp;
            }
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

        public List<CheckBoxes> SettingCheckBoxes(string currentUserEmail, string bigCategory)
        {
            bool hasFavorites = false;
            List<string> bigListSelection = BigCategories(bigCategory);
            List<Favorite> favoriteList = new List<Favorite>();

            if (currentUserEmail == null)                            //first time log in or no log in
            {
                currentUserEmail == "abcdefg@abcdefg.com;"
            }

            IQueryable<Favorite> favQuery = db.Favorites.AsQueryable();     //favorites search in DB
            favQuery = favQuery.Where(x => x.Email == currentUserEmail);


            favoriteList = favQuery.ToList();                               //to List and are there favorites?
            List<CheckBoxes> favBox = new List<CheckBoxes>();
            if (favoriteList.Count != 0)
            {

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
                hasFavorites = true;
                
            }
            else
            {
                hasFavorites = false;
                
            }

            if (bigListSelection.Count == 0)                                        //if selection is 0 then return just the favorites
            {
                if(hasFavorites == true)                                            //if the favorite loop ran returns favlist else it returns everything unchecked
                {
                    return favBox; 
                }
                else
                {
                    favBox = AddingCatsToList();
                    return AddingCatsToList();
                }
            }
            else
            {                                                                       //if the category is selected
                if (hasFavorites == false)                                          //seeing is the favorites loop ran if it didnt it populates all list  
                {
                    favBox = AddingCatsToList();
                }

                bool isMatch = false;                                               //if a match is found in the loop below it will break out of the seconed loop
                List<CheckBoxes> bigCat = new List<CheckBoxes>();
                
                foreach (string big in bigListSelection)                            //selection from bigCategories
                {
                    isMatch = false;

                    foreach (CheckBoxes box in favBox)                              //adding true values from the all list or fav list to new big cat list
                    {

                        if (big == box.Name && !bigCat.Any(x => x.Name == box.Name))
                        {
                            isMatch = true;
                            bigCat.Add(new CheckBoxes() { IsCheck = true, CheckName = box.CheckName, Name = box.Name, Value = box.Value });
                        }

                        if (isMatch == true)
                        {
                            break;
                        }
                    }
                }

                foreach(CheckBoxes box in favBox)                                   //with the added true values, add the rest of the values if they are not in the list
                {
                    if (!bigCat.Any(x => x.Name == box.Name))
                    {
                        bigCat.Add(new CheckBoxes() { IsCheck = box.IsCheck, CheckName = box.CheckName, Name = box.Name, Value = box.Value });
                    }
                }
                return bigCat;
            }
        }

        public ActionResult SaveFavorties(Favorite newFavorite)
        {
            DataContext db = new DataContext();
            db.Favorites.Add(newFavorite);
            db.SaveChanges();
            return RedirectToAction("Favorites"); 
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

