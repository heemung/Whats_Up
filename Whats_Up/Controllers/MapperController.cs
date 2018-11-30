using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Whats_Up.Controllers
{
    public class MapperController : Controller 
        
        //http://dev.virtualearth.net/REST/v1/Locations/42.453492, -83.193713?key=AtFQgc_suTzYYJXUJvNrdB0asoD6z2P3dhJewKOVbNM9I3vWtwbr6dXP16IB_0QK
    {
        // GET: Mapper
        public ActionResult Index()
        {
            string bing = WebConfigurationManager.AppSettings["Mapper"];

            UriBuilder builder = new UriBuilder
            {
                Scheme = "http",
                Host = "dev.virtualearth.net",
                Path = "REST/v1/Locations/42.453492,-83.193713",
                Query = "key=" + bing,
            };

            HttpWebRequest requestLongLat = WebRequest.CreateHttp(builder.ToString());
            requestLongLat.UserAgent = "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 64.0) Gecko / 20100101 Firefox / 64.0";

            HttpWebResponse reponse = (HttpWebResponse)requestLongLat.GetResponse();

            if (reponse.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(reponse.GetResponseStream());

                string output = reader.ReadToEnd();
                JObject jLocationObject = JObject.Parse(output);

                ViewBag.ThisTest = jLocationObject;
                ViewBag.ThisTest2 = builder.ToString();
            }

            return View();
        }
       
    }
}
    