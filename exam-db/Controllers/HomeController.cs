using exam_db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Web.Routing;

namespace exam_db.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            if (Session["lang"] != null)
            {
                string test = Session["lang"].ToString();
            }
            ViewBag.college = db.Colleges.ToList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Search()
        {

            return View();
        }

        public new ActionResult Profile()
        {
            return View();
        }

        public ActionResult ChangeLang(string lang)
        {
            string[] link = ReferrerUrl();
            Session.Remove("lang");
            if (lang == "ar")
            {
                string culture = "ar";
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                if (Session != null)
                {
                    Session["lang"] = "ar";
                }
                                   
            }
            else
            {
                string culture = "en-US";
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                if (Session != null)
                {
                    Session["lang"] = "en";
                }
                   
            }
            ViewBag.college = db.Colleges.ToList();
            return RedirectToAction(link[1], link[0]);
        }


        public string[] ReferrerUrl()
        {
            var fullUrl = Request.UrlReferrer.ToString();
            var questionMarkIndex = fullUrl.IndexOf('?');
            string queryString = null;
            string url = fullUrl;
            if (questionMarkIndex != -1) // There is a QueryString
            {
                url = fullUrl.Substring(0, questionMarkIndex);
                queryString = fullUrl.Substring(questionMarkIndex + 1);
            }

            // Arranges
            var request = new HttpRequest(null, url, queryString);
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);

            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            // Extract the data    
            var values = routeData.Values;
            var controllerName = values["controller"].ToString();
            var actionName = values["action"].ToString();
            var areaName = values["area"];

            string[] link = new string[2];
            link[0] = controllerName;
            link[1] = actionName;

            return link;
        }
        
    }
}