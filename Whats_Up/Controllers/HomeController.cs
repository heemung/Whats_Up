using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;

namespace Whats_Up.Controllers
{
    public class HomeController : Controller
    {
        public string BingKey = WebConfigurationManager.AppSettings["Mapper"];
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WhatsUp()
        {
            ViewBag.Message = "Your application description page.";

            
            ViewBag.Key = BingKey;

            return View();
        }
        public ActionResult InputLocation(string addressLine, string postalCode)
        {
            string URL = "http://dev.virtualearth.net/REST/v1/Locations/US/adminDistrict/postalCode/locality/addressLine?" +
                "addressLine="+addressLine+"&postalCode="+postalCode+"&maxResults=5&key="+BingKey;
            HttpWebRequest request = WebRequest.CreateHttp(URL);
            request.UserAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64; rv: 47.0) Gecko / 20100101 Firefox / 47.0";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string output = reader.ReadToEnd();
                JObject JParser = JObject.Parse(output);
            }
            return View();
        }

        public ActionResult Creators()
        {
            ViewBag.Message = "Your 2018 C# .NET Developers";

            return View();
        }
    }
}