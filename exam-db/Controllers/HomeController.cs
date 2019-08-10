using exam_db.Models;
using exam_db.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace exam_db.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Application DB context
        /// </summary>
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }
        public HomeController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        #region 
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
        public List<RecommendViewModel> Get (string id)
        {
            List<RecommendViewModel> li = new List<RecommendViewModel>();
            string query = @"select i.Id as 'id',i.title as 'title',d.name as 'department',u.UserName 'as name',i.likeNumber as 'like',i.viewNumber as 'view',i.yearOfPublish as'year',i.downloadNumber as 'download'from Items i inner join Courses c on i.CourseId=c.Id inner join Departments d on c.departmentId=d.Id
                        inner join AspNetUsers u on u.departmentId=d.Id where u.Id='" + id+"'";
            SqlCommand cmd = new SqlCommand(query, Connection.Get());
            SqlDataReader srd = cmd.ExecuteReader();
            while (srd.HasRows)
            {
                RecommendViewModel r = new RecommendViewModel();
                r.Id = (int)srd["id"];
                r.title = (string)srd["title"];
                r.department = (string)srd["department"];
                r.user = (string)srd["name"];
                r.views = (int)srd["view"];
                r.year = (string)srd["year"];
                r.download = (int)srd["download"];
                r.likes = (int)srd["like"];
                li.Add(r);

            }
            srd.Close();
            

            //li = from c in db.Items
            //     join cn in db.Courses on c.CourseId equals cn.Id
            //     join d in db.Departments on cn.departmentId equals d.Id
            //     join user in db.Users on d.Id equals user.Id
            //     where user.Id = Request.LogonUserIdentity.GetUserId()
            //     select new { Id = c.Id }; return li.ToList();

            return li;
        }
        #endregion
        public ActionResult Recommended()
        {
            if (Request.IsAuthenticated)
            {
                string userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(x => x.Id == userId);
                //var user = db.Users.Where(x => x.Id == userId).SingleOrDefault();
                IEnumerable<RecommendViewModel> response = db.Items.Where(x => x.Course.departmentId == user.departmentId).OrderByDescending(x => x.likeNumber).Take(5).Select(x => new RecommendViewModel() { Id = x.Id, title = x.title, college = x.Course.department.college.name, department = x.Course.name, download = x.downloadNumber, likes = x.likeNumber, user = x.User.UserName, views = x.viewNumber, year = x.yearOfPublish }).ToList();
                return View(response);
            }
            return View();
          
        }
        [HttpGet]
        public ActionResult Search(Search s, int? page)
        {
            List<Item> response = null;
            int pagesize = 10, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Item> li = null;
            if (s != null)
            {
                if (s.search != null && s.all == null && s.final == null && s.quiz == null && s.summary == null && s.year == null &&s.mid==null) //Only Search without filters
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
                    if (s.year==null)
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
                            response = db.Items.Where(x => x.category == s.summary ).ToList();
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
                            if (s.year=="allyear")
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
                        else if (s.year=="allyears")
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
                    if (s.year==null)
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
                        else if (s.year=="allyears")
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
                else if (s.year=="allyears")
                {
                    if (s.search==null)
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
                            response = db.Items.Where(x => x.category == s.mid && x.title.Contains(s.search) || x.FileContent.Contains(s.search) ).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.final != null)
                        {
                            response = db.Items.Where(x => x.category == s.final && x.title.Contains(s.search) || x.FileContent.Contains(s.search) ).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.summary != null)
                        {
                            response = db.Items.Where(x => x.category == s.summary && x.title.Contains(s.search) || x.FileContent.Contains(s.search) ).ToList();
                            li = response.ToPagedList(pageindex, pagesize);
                        }
                        else if (s.quiz != null)
                        {
                            response = db.Items.Where(x => x.category == s.quiz && x.title.Contains(s.search) || x.FileContent.Contains(s.search) ).ToList();
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
        public ActionResult Index()
        {
            ViewBag.college = db.Colleges.ToList();
            SelectList li = new SelectList(db.Departments.ToList(), "Id", "name");
            ViewBag.lst = li;

            return View();
        }
        [HttpPost]
        public void SendFile(IEnumerable<HttpPostedFileBase> fileData)
        {
            TempData["DragFileData"] = fileData;
            TempData.Keep();
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
                string extention = Path.GetExtension(item.FileName);
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
                    model.UserId =System.Web.HttpContext.Current.User.Identity.GetUserId(); //To Re-edit when login auths are done

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

        public ActionResult Profile()
        {
            return View();
        }

    }
}