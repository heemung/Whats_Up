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
                Path = "/rest/v1/satellite/positions/25544//1/1/0/2/&apiKey=" + SatTracker,
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