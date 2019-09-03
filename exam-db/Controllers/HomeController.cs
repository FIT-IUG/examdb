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
using System.Data;
using System.Data.Entity;
using PagedList;
using exam_db.ViewModels;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace exam_db.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }

        private const int defaultPageSize = 8;
        private ApplicationDbContext db;
        public HomeController()
        {
            this.db = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(this.db));
        }

        [Authorize]
        public ActionResult Index()
        {
            if (Session["lang"] != null)
            {
                string test = Session["lang"].ToString();
            }
            ViewBag.Colleges = db.Colleges.ToList();
            return View();
        }

        public JsonResult getCourses(String idString)

        {

            int i = 0;

            Int32.TryParse(idString, out i);

            db.Configuration.ProxyCreationEnabled = false;

            var courses = db.Courses.Where(d => d.departmentId == i).ToList();

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        public string ReadText(List<ReadContent> paths)
        {
            ConvertText c = new ConvertText();
            string x = "";
            foreach (var item in paths)
            {
                if (item.extention == ".pdf")
                {
                    x += c.ToPdf(item);
                }
                else if (item.extention == ".docx")
                {
                    x += c.GetTextFromWord(item);
                }
                else if (item.extention == ".xlsx")
                {
                    x += c.XLS(item);
                }
                else if (item.extention == ".csv")
                {
                    x += c.CSV(item);
                }
                else if (item.extention == ".txt")
                {
                    x += c.TXT(item);
                }
            }

            return x;
        }
        public ActionResult Upload(Item model, HttpPostedFileBase[] mfile)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {





                string[] arr = { ".pdf", ".csv", ".jpg", ".png", ".ppt", ".txt", ".xlsx", ".docx", ".doc", "xls" };
                foreach (var item in mfile)
                {
                    string extention = Path.GetExtension(item.FileName.ToLower());
                    if (arr.Contains(extention))
                    {
                        continue;
                    }
                    else
                    {
                        return Content(" Extentions Not Allowed !!! "); /*HttpStatusCodeResult(HttpStatusCode.BadRequest)*/;


                    }
                }
                try
                {
                    model.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId(); //To Re-edit when login auths are done

                    model.uploadTime = DateTime.Now;
                    //model.category = "";
                    model.likeNumber = 0;
                    model.viewNumber = 0;
                    // model.yearOfPublish = model.yearOfPublish;
                    model.downloadNumber = 0;
                    List<ReadContent> paths = new List<ReadContent>();
                    //  model.uploadTime = new DateTime(2008, 07, 1, 23, 25, 30);
                    db.Items.Add(model);
                    db.SaveChanges();
                    //Collection of Paths
                    foreach (var item in mfile)
                    {


                    }

                    //End of Collection of Paths

                    foreach (var item in mfile)
                    {

                        Models.File f = new Models.File();
                        f.itemId = model.Id;
                        f.path = uploadFile(item);
                        ReadContent r = new ReadContent();
                        r.path = Server.MapPath(f.path);
                        r.extention = Path.GetExtension(f.path);
                        paths.Add(r);

                        //Uploading logic continues......
                        f.size = (int)item.ContentLength;
                        if (item.ContentType == "text/csv")
                        {
                            f.type = FileType.csv;
                        }
                        else if (item.ContentType == "text/plain")
                        {
                            f.type = FileType.txt;
                        }
                        else if (item.ContentType == "image/png")
                        {
                            f.type = FileType.png;
                        }
                        else if (item.ContentType == "image/jpg" || item.ContentType == "image/jpeg")
                        {
                            f.type = FileType.Jpg;
                        }
                        else if (item.ContentType == "application/pdf")
                        {
                            f.type = FileType.Pdf;
                        }
                        else if (item.ContentType == "application/vnd.ms-powerpoint")
                        {
                            f.type = FileType.ppt;
                        }
                        else if (item.ContentType == "application/vnd.ms-excel")
                        {
                            f.type = FileType.xls;
                        }
                        else if (item.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                        {
                            f.type = FileType.docx;
                        }

                        else
                        {
                            f.type = FileType.lba;

                        }

                        db.Files.Add(f);
                        db.SaveChanges();
                        string content = ReadText(paths);
                        model.FileContent = content;
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");

                }


                catch (DbUpdateException e)
                {

                    var sqlException = e.GetBaseException() as SqlException;
                    if (sqlException != null)
                    {
                        if (sqlException.Errors.Count > 0)
                        {
                            switch (sqlException.Errors[0].Number)
                            {
                                case 547: // Foreign Key violation
                                    ModelState.AddModelError("CodeInUse", "Country code could not be deleted, because it is in use");
                                    return RedirectToAction("Index");
                                default:
                                    throw;
                            }
                        }
                    }


                    return RedirectToAction("Index");

                }
            }
        }

        private string uploadFile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);

                // try
                // {

                path = Path.Combine(Server.MapPath("~/Content/uploads/"), random + Path.GetFileName(file.FileName));
                file.SaveAs(path);
                path = "~/Content/uploads/" + random + Path.GetFileName(file.FileName);

                //    ViewBag.Message = "File uploaded successfully";
                // }
                //catch (Exception)
                //{
                //    path = "-1";
                //}


            }

            else
            {
                Response.Write("<script> alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
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
        public ActionResult deleteFromHistory(int id) {
            string[] existingQuestions = (string[])System.Web.HttpContext.Current.Session["Questions"];
            String[] newQuestions = existingQuestions.Where(val => val != existingQuestions[id]).ToArray(); 
            System.Web.HttpContext.Current.Session["Questions"] = newQuestions;
            string[] existingUrls = (string[])System.Web.HttpContext.Current.Session["urls"];
            String[] newUrl = existingUrls.Where(val => val != existingUrls[id]).ToArray(); ;

            System.Web.HttpContext.Current.Session["urls"] = newUrl;
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Search(Search s, int? page)
        {
            string[] Questions = { s.search };
            String[] urls = { Request.Url.ToString() };
            if (System.Web.HttpContext.Current.Session["Questions"] == null)
            {
                System.Web.HttpContext.Current.Session["Questions"] = Questions; // here question is string array, 
                                                                                 //assigning value of array to session if session is null
                System.Web.HttpContext.Current.Session["urls"] = urls;
            }
            else
            {
                string[] newQuestions = Questions;
                string[] existingQuestions = (string[])System.Web.HttpContext.Current.Session["Questions"];
                System.Web.HttpContext.Current.Session["Questions"] = newQuestions.Union(existingQuestions).ToArray();

                string[] newUrl = urls;
                string[] existingUrls = (string[])System.Web.HttpContext.Current.Session["urls"];
                System.Web.HttpContext.Current.Session["urls"] = newUrl.Union(existingUrls).ToArray();
            }

            List<Item> response = null;
            int pagesize = 10, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Item> li = null;
            if (s != null)
            {
                if (s.search != null && s.all == null && s.final == null && s.quiz == null && s.summary == null && s.year == null && s.mid == null) //Only Search without filters
                {
                    response = db.Items.Where(x => x.FileContent.Contains(s.search) || x.title.Contains(s.search)).ToList();
                    li = response.ToPagedList(pageindex, pagesize);
                }
                else if (s.search == null)
                {
                    //if (s.all!=null && s.year==null)
                    //{
                    //    response = db.Items.ToList();
                    //    li = response.ToPagedList(pageindex, pagesize);
                    //}
                    if (s.year == null)
                    {
                        if (s.mid != null)
                        {
                            response = db.Items.Where(x => x.category == s.mid).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.final != null)
                        {
                            response = db.Items.Where(x => x.category == s.final).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.summary != null)
                        {
                            response = db.Items.Where(x => x.category == s.summary).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.quiz != null)
                        {
                            response = db.Items.Where(x => x.category == s.quiz).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else
                        {
                            response = db.Items.ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                    }
                    else
                    {
                        if (s.mid != null)
                        {
                            if (s.year == "allyear")
                            {
                                response = db.Items.Where(x => x.category == s.mid).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }
                            else
                            {
                                response = db.Items.Where(x => x.category == s.mid && x.yearOfPublish == s.year).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }

                        }
                        else if (s.final != null)
                        {
                            if (s.year == "allyear")
                            {
                                response = db.Items.Where(x => x.category == s.final).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }
                            else
                            {
                                response = db.Items.Where(x => x.category == s.final && x.yearOfPublish == s.year).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }

                        }
                        else if (s.summary != null)
                        {
                            if (s.year == "allyear")
                            {
                                response = db.Items.Where(x => x.category == s.summary).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }
                            else
                            {
                                response = db.Items.Where(x => x.category == s.summary && x.yearOfPublish == s.year).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }

                        }
                        else if (s.quiz != null)
                        {
                            if (s.year == "allyear")
                            {
                                response = db.Items.Where(x => x.category == s.quiz).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }
                            else
                            {
                                response = db.Items.Where(x => x.category == s.quiz && x.yearOfPublish == s.year).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }

                        }
                        else if (s.year == "allyears")
                        {
                            response = db.Items.ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else
                        {
                            response = db.Items.Where(x => x.yearOfPublish == s.year).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                    }
                }
                else if (s.search != null)
                {
                    if (s.year == null)
                    {

                        if (s.mid != null)
                        {
                            response = db.Items.Where(x => x.category == s.mid && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.final != null)
                        {
                            response = db.Items.Where(x => x.category == s.final && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.summary != null)
                        {
                            response = db.Items.Where(x => x.category == s.summary && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.quiz != null)
                        {
                            response = db.Items.Where(x => x.category == s.quiz && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else
                        {
                            response = db.Items.Where(x => x.FileContent.Contains(s.search) || x.title.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                    }
                    else
                    {
                        if (s.mid != null)
                        {
                            if (s.year == "allyear")
                            {
                                response = db.Items.Where(x => x.category == s.mid && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }
                            else
                            {
                                response = db.Items.Where(x => x.category == s.mid && x.title.Contains(s.search) || x.FileContent.Contains(s.search) && x.yearOfPublish == s.year).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }


                        }
                        else if (s.final != null)
                        {
                            if (s.year == "allyear")
                            {
                                response = db.Items.Where(x => x.category == s.final && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }
                            else
                            {
                                response = db.Items.Where(x => x.category == s.final && x.title.Contains(s.search) || x.FileContent.Contains(s.search) && x.yearOfPublish == s.year).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }

                        }
                        else if (s.summary != null)
                        {
                            if (s.year == "allyear")
                            {
                                response = db.Items.Where(x => x.category == s.summary && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }
                            else
                            {
                                response = db.Items.Where(x => x.category == s.summary && x.title.Contains(s.search) || x.FileContent.Contains(s.search) && x.yearOfPublish == s.year).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }

                        }
                        else if (s.quiz != null)
                        {
                            if (s.year == "allyear")
                            {
                                response = db.Items.Where(x => x.category == s.quiz && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }
                            else
                            {
                                response = db.Items.Where(x => x.category == s.quiz && x.title.Contains(s.search) || x.FileContent.Contains(s.search) && x.yearOfPublish == s.year).ToList();
                                li = response.ToPagedList(pageindex, pagesize);
                            }

                        }
                        else if (s.year == "allyears")
                        {
                            response = db.Items.Where(x => x.FileContent.Contains(s.search) || x.title.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else
                        {
                            response = db.Items.Where(x => x.FileContent.Contains(s.search) || x.title.Contains(s.search) && x.yearOfPublish == s.year).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                    }

                }
                else if (s.year == "allyears")
                {
                    if (s.search == null)
                    {
                        if (s.mid != null)
                        {
                            response = db.Items.Where(x => x.category == s.mid).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.final != null)
                        {
                            response = db.Items.Where(x => x.category == s.final).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.summary != null)
                        {
                            response = db.Items.Where(x => x.category == s.summary).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.quiz != null)
                        {
                            response = db.Items.Where(x => x.category == s.quiz).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else
                        {
                            response = db.Items.ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                    }
                    else
                    {
                        if (s.mid != null)
                        {
                            response = db.Items.Where(x => x.category == s.mid && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.final != null)
                        {
                            response = db.Items.Where(x => x.category == s.final && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.summary != null)
                        {
                            response = db.Items.Where(x => x.category == s.summary && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.quiz != null)
                        {
                            response = db.Items.Where(x => x.category == s.quiz && x.title.Contains(s.search) || x.FileContent.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else
                        {
                            response = db.Items.Where(x => x.FileContent.Contains(s.search) || x.title.Contains(s.search)).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                    }

                }


                return View(li);
            }

            ViewBag.message = "No Result";
            return View();
        }
        public ActionResult Recommended()
        {
            if (Request.IsAuthenticated)
            {
                string userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                //var user = db.Users.Where(x => x.Id == userId).SingleOrDefault();
                IEnumerable<RecommendViewModel> response = db.Items.Where(x => x.Course.departmentId == user.departmentId).OrderByDescending(x => x.likeNumber).Take(5).Select(x => new RecommendViewModel() { Id = x.Id, title = x.title, college = x.Course.department.college.name, department = x.Course.name, download = x.downloadNumber, likes = x.likeNumber, user = x.User.UserName, views = x.viewNumber, year = x.yearOfPublish, listOfFile = x.listOfFile }).ToList();
                return PartialView(response);
            }
            return PartialView();

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
            if(!string.IsNullOrEmpty(user.firstname) && !string.IsNullOrEmpty(user.lastname))
            {
                if (!string.IsNullOrEmpty(user.firstname.Trim()) && !string.IsNullOrEmpty(user.lastname.Trim()))
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
                }
            }
            jsonResult = Json(new
            {
                success = false,
                error = "FirstName & LastName can't be Empty"
            }, JsonRequestBehavior.AllowGet);
            return jsonResult;
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

        public ActionResult GetUploadedItems(int pageIndex, int method) // method indicate getting the (1 -> uploaded or 2-> Favorites) items 
        {
           
            var userId = User.Identity.GetUserId();
            List<Item> items = new List<Item>();
            List<Object> objects = new List<Object>();
            List<Item> db_list = new List<Item>();
            if (method == 1)
            {
                db_list = db.Items.Where(c => c.UserId == userId).ToList();
            }
            else if(method == 2)
            {
                var favorite_items = db.Favorites.Include(f => f.Item).Where(f => f.UserId == userId).ToList();
                foreach (Favorite favorite in favorite_items)
                {
                    Item item = favorite.Item;
                    db_list.Add(item);
                }
            }

            Models.Paging paging = new Models.Paging();
            
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
            return Redirect(Request.UrlReferrer.ToString());
            //return RedirectToAction(link[1], link[0]);
        }

        /* getting the controller & the action names for the Referrer url */
        //public string[] ReferrerUrl()
        //{
        //    var fullUrl = Request.UrlReferrer.ToString();
        //    var questionMarkIndex = fullUrl.IndexOf('?');
        //    string queryString = null;
        //    string url = fullUrl;
        //    if (questionMarkIndex != -1) // There is a QueryString
        //    {
        //        url = fullUrl.Substring(0, questionMarkIndex);
        //        queryString = fullUrl.Substring(questionMarkIndex + 1);
        //    }

        //    // Arranges
        //    var request = new HttpRequest(null, url, queryString);
        //    var response = new HttpResponse(new StringWriter());
        //    var httpContext = new HttpContext(request, response);

        //    RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

        //    // Extract the data    
        //    var values = routeData.Values;
        //    var controllerName = values["controller"].ToString();
        //    var actionName = values["action"].ToString();
        //    var areaName = values["area"];

        //    string[] link = new string[3];
        //    link[0] = controllerName;
        //    link[1] = actionName;
        //    link[2] = fullUrl;
        //    return link;
        //}
        
    }
}