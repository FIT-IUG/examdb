using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using exam_db.Models;

namespace exam_db
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<ApplicationDbContext>(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session == null)
            {
                string culture = "en-US";
                if (Request.UserLanguages != null)
                {
                    culture = Request.UserLanguages[0];
                }

                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                if (Thread.CurrentThread.CurrentCulture.ToString().Contains("ar"))
                {
                    if (HttpContext.Current.Session != null)
                    {
                        /*use something elss - problem*/
                        HttpContext.Current.Session["lang"] = "ar";
                    }
                }

            }else
            {
                if(HttpContext.Current.Session["lang"] != null)
                {
                    if(HttpContext.Current.Session["lang"].ToString() == "ar")
                    {
                        string culture = "ar";
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                        HttpContext.Current.Session["lang"] = "ar";
                    }else
                    {
                        string culture = "en";
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                        HttpContext.Current.Session["lang"] = "en";
                    }
                }
            }
            

           
        }
    }
}
