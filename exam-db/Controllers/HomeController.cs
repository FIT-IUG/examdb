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
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using PagedList;

namespace exam_db.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        private const int defaultPageSize = 1;
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

        [Authorize]
        public new ActionResult Profile()
        {
            ViewBag.college = db.Colleges.ToList();
            if (User != null)
            {
                var username = User.Identity.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var user = db.Users.SingleOrDefault(u => u.UserName == username);
                    ViewBag.User =  user;
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult ProfileEditName(ApplicationUser user)
        {
            JsonResult jsonResult = new JsonResult();
            jsonResult.MaxJsonLength = Int32.MaxValue;

            if (!string.IsNullOrEmpty(user.firstname) && !string.IsNullOrEmpty(user.lastname))
            {
                var username = User.Identity.Name;
                var user_db = db.Users.SingleOrDefault(u => u.UserName == username);
                user_db.firstname = user.firstname;
                user_db.lastname = user.lastname;
                db.SaveChanges();
                jsonResult = Json(new
                {
                    success = true
                }, JsonRequestBehavior.AllowGet);
                return jsonResult;
            }else
            {
                jsonResult = Json(new
                {
                    success = false,
                    error = "FirstName & LastName can't be Empty"
                }, JsonRequestBehavior.AllowGet);
                return jsonResult;
            }
        }

        [HttpPost]
        public ActionResult ProfileEditCollege(ApplicationUser user)
        {
            JsonResult jsonResult = new JsonResult();
            jsonResult.MaxJsonLength = Int32.MaxValue;

            if (user.departmentId != 0)
            {
               
                var username = User.Identity.Name;
                var user_db = db.Users.SingleOrDefault(u => u.UserName == username);
                user_db.departmentId = user.departmentId;
                db.SaveChanges();
                jsonResult = Json(new
                {
                    success = true
                }, JsonRequestBehavior.AllowGet);
                return jsonResult;
            }
            else
            {
                jsonResult = Json(new
                {
                    success = false,
                    error = "You Must Select the Department"
                }, JsonRequestBehavior.AllowGet);
                return jsonResult;
            }
        }

        public ActionResult GetUploadedItems(int pageIndex)
        {
            var userId = User.Identity.GetUserId();
            List<Item> items = new List<Item>();
            List<Object> objects = new List<Object>();
            var db_list = db.Items.Where(c => c.UserId == userId).ToList();

            Paging paging = new Paging();
            String jsonString = "";
            int offset = 1;
            int skip = 0;
            int Take = defaultPageSize;
            if (pageIndex == 1)
                skip = 0;
            else
                skip = ((pageIndex - 1) * Take);

            int total = db_list.Count;
            var data = db_list.Skip(skip).Take(Take);
            int startIndex = (pageIndex - 1) * defaultPageSize;

            foreach (Item item in data)
            {
                Item itm = new Item();
                itm.Id = item.Id;
                itm.title = item.title;
                itm.likeNumber = item.likeNumber;
                itm.viewNumber = item.viewNumber;
                itm.downloadNumber = item.downloadNumber;
                itm.yearOfPublish = item.yearOfPublish;
                itm.uploadTime = item.uploadTime;
                itm.CourseName = item.Course.name;
                itm.DepartmentName = item.Course.department.name;
                Object myObject = new Item() { Id = item.Id, title = item.title, likeNumber = item.likeNumber, viewNumber = item.viewNumber, downloadNumber = item.downloadNumber, yearOfPublish = item.yearOfPublish, uploadTime = item.uploadTime, CourseName = item.Course.name, DepartmentName = item.Course.department.name};
                objects.Add(myObject);
                jsonString = JsonConvert.SerializeObject(itm, Formatting.None);
            }
            string pagin = paging.Pagination(total, pageIndex, Take, offset, "GetUploadedItems", "Profile", "");
            int totalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(total) / defaultPageSize));
            ViewBag.total = totalPage;
            var list = JsonConvert.SerializeObject(objects, Formatting.Indented);
            return Json(new
            {
                data = list,
                totalPage = totalPage
            }, JsonRequestBehavior.AllowGet);


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


        /* getting the controller & the action names for the Referrer url */
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