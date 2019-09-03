using exam_db.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using Ionic.Zip;
using System.IO.Compression;

namespace exam_db.Controllers
{
    public class WebController : ApplicationBaseController
    {
        private const int defaultPageSize = 5;
        private IList<Course> allCourse = new List<Course>();
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult College(int CollegeId)
        {
            College college = db.Colleges.Find(CollegeId);
            ViewBag.CollegeName = college.name;
            //ViewBag.CollegeId = college.Id;
            ViewBag.Departments = college.listOfDepartment.Skip(1);
            //ViewBag.DepartmentsCount = college.listOfDepartment.Count;
            Department firstdept = college.listOfDepartment.First();
            //ViewBag.listOfCourses = firstdept.listOfCourse;
            //ViewBag.listOfIds = college.listOfDepartment;
            ViewBag.First = college.listOfDepartment.First();
            Models.Paging paging = new Models.Paging();
            int offset = 1;
            int Page = 1;
            int Take = defaultPageSize;
            if (Convert.ToInt32(Request.QueryString["Page"]) > 1)
            {
                Page = Convert.ToInt32(Request.QueryString["Page"]);
            }

            int skip = 0;
            if (Page == 1)
                skip = 0;
            else
                skip = ((Page - 1) * Take);
            int total = firstdept.listOfCourse.Count();
            var data = firstdept.listOfCourse.Skip(skip).Take(Take);
            string pagin = paging.Pagination(total, Page, Take, offset, "College", "College", "");
            ViewBag.Paging = pagin;
            return View(data.ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Getdept(int id, int pageIndex)
        {
            List<Course> courses = new List<Course>();
            List<Object> Mycourses = new List<Object>();
            Department dep = db.Departments.Single(a => a.Id.Equals(id));
            //Department model = new Department();
            //JsonArrayAttribute coursesArray = new JsonArrayAttribute();
            Models.Paging paging = new Models.Paging();
            String myjson = "";
            int offset = 1;
            int skip = 0;
            int Take = defaultPageSize;
            if (pageIndex == 1)
                skip = 0;
            else
                skip = ((pageIndex - 1) * Take);
            int total = dep.listOfCourse.Count();
            var data = dep.listOfCourse.Skip(skip).Take(Take);
            int startIndex = (pageIndex - 1) * defaultPageSize;
            //IQueryable<ICollection<Course>> listOfCourses = from q in db.Departments where q.Id == data.depid select q.listOfCourse;
            foreach (Course course in data)
            {
                Course mycourse = new Course();
                mycourse.Id = course.Id;
                mycourse.name = course.name;
                mycourse.code = course.code;
                mycourse.listOfItem = course.listOfItem;
                mycourse.departmentId = course.departmentId;
                Object myObject = new
                {
                    course = new Course() { Id = course.Id, name = course.name, code = course.code, departmentId = course.departmentId }
                };
                Mycourses.Add(myObject);
                myjson = JsonConvert.SerializeObject(mycourse, Formatting.None);
            }
            string pagin = paging.Pagination(total, pageIndex, Take, offset, "Getdept", "/College", "");
            int totalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(total) / defaultPageSize));
            ViewBag.total = totalPage;
            var list = JsonConvert.SerializeObject(courses,
                                    Formatting.None,
                                    new JsonSerializerSettings()
                                    {
                                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                    });
            var list2 = JsonConvert.SerializeObject(Mycourses, Formatting.Indented);
            //string responseText = JSON.Validate(courses);
            //return Json( list , "application/json") ;
            return Json(new
            {
                data = list2,
                totalPage = totalPage
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Course(int courseId, String CollegeName)
        {
            Course course = db.Courses.Find(courseId);
            ViewBag.CourseId = courseId;
            ViewBag.courseName = course.name;
            ViewBag.ListOfile = course.listOfItem;
            Models.Paging paging = new Models.Paging();
            int offset = 1;
            int Page = 1;
            int Take = defaultPageSize;
            if (Convert.ToInt32(Request.QueryString["Page"]) > 1)
            {
                Page = Convert.ToInt32(Request.QueryString["Page"]);
            }
            int skip = 0;
            if (Page == 1)
                skip = 0;
            else
                skip = ((Page - 1) * Take);
            int total = course.listOfItem.Count();
            var data = course.listOfItem.Where(a => a.category.Equals("Exams")).Skip(skip).Take(Take);
            string pagin = paging.Pagination(total, Page, Take, offset, "Course", "/Course", "");
            List<Models.File> exams = new List<Models.File>();
            ViewBag.exams = exams;
            ViewBag.CollegeName = CollegeName;
            Department department = db.Departments.Find(course.departmentId);
            ViewBag.departmentName = department.name;
            ViewBag.Paging = pagin;
            return View(data.ToList());
        }
        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        public ActionResult GetFiles(String filetype, int courseId, int pageIndex)
        {
            Course course = db.Courses.Find(courseId);
            List<Object> MyFiles = new List<Object>();
            int offset = 1;
            int skip = 0;
            int Take = 10;
            if (pageIndex == 1)
                skip = 0;
            else
                skip = ((pageIndex - 1) * Take);
            int total = course.listOfItem.Count();
            var data = course.listOfItem.Skip(skip).Take(Take);
            int startIndex = (pageIndex - 1) * defaultPageSize;
            foreach (Item item in data)
            {
                if (item.category.Equals(UppercaseFirst(filetype)))
                {
                    Item myItem = new Item();
                    myItem.category = item.category;
                    myItem.Course = item.Course;
                    myItem.CourseId = item.CourseId;
                    myItem.downloadNumber = item.downloadNumber;
                    myItem.Id = item.Id;
                    myItem.likeNumber = item.likeNumber;
                    myItem.semester = item.semester;
                    myItem.title = item.title;
                    myItem.examType = item.examType;
                    myItem.viewNumber = item.viewNumber;
                    myItem.yearOfPublish = item.yearOfPublish;
                    Object myobject = new
                    {
                        file = new Item()
                        {
                            Id = myItem.Id,
                            category = myItem.category,
                            downloadNumber = myItem.downloadNumber,
                            likeNumber = myItem.likeNumber,
                            semester = myItem.semester,
                            title = myItem.title,
                            examType = myItem.examType,
                            viewNumber = myItem.viewNumber,
                            yearOfPublish = myItem.yearOfPublish
                        }
                    };
                    MyFiles.Add(myobject);
                }
            }
            int totalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(total) / defaultPageSize));
            var fileList = JsonConvert.SerializeObject(MyFiles, Formatting.Indented);
            return Json(new
            {
                data = fileList,
                totalPage = totalPage
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult File(int fileId)
        {
            Item item = db.Items.Find(fileId);
            ViewBag.CollegeName = item.Course.department.college.name;
            ViewBag.depName = item.Course.department.name;

            int count = (from a in db.Favorites where a.ItemId == fileId select a).Count();
            item.likeNumber = count;
            String userId = User.Identity.GetUserId(); // get Current User
            Favorite existFav = (from a in db.Favorites where a.UserId == userId && a.ItemId == fileId select a).FirstOrDefault();
            if (existFav == null)
            {
                ViewBag.Liked = "false";
            }
            else
            {
                ViewBag.Liked = "true";
            }

            //get Reports Constants 
            Constant constant = (from a in db.Constants where a.Key == "Report" select a).FirstOrDefault();
            List<Constant> constants = (from a in db.Constants where a.parentId == constant.Id select a).ToList();
            ViewBag.constants = constants;

            //get Related Files
            List<Item> relatedFiles = (from a in db.Items where a.CourseId == item.CourseId && a.Id != item.Id select a).OrderByDescending(v => v.downloadNumber).Take(5).ToList();
            //List<Item> relatedFiles = db.Items.Where(a => a.CourseId == item.CourseId && a.Id == item.Id).Take(2).ToList();
            Course course = db.Courses.Find(item.CourseId);
            if (relatedFiles.Count() == 0)
            {
                relatedFiles = (from a in db.Items where a.Course.departmentId == course.departmentId && a.Id != item.Id select a).OrderByDescending(v => v.downloadNumber).Take(5).ToList();
            }
            ViewBag.Related = relatedFiles;

            // create Cookies 

            string cookie = userId + item.Id;

            if (Request.Cookies[cookie] == null)
            {
                HttpCookie view = new HttpCookie(cookie);
                view[""] = cookie;
                view.Expires.AddMonths(2);
                Response.Cookies.Add(view);

                item.viewNumber = item.viewNumber + 1;
                if (ModelState.IsValid)
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                //ViewBag.user = cookie;
                ViewBag.user = Request.Cookies[cookie].Value;
            }

            long filesSize = 0;

            //get size of files in each item
            foreach (Models.File file in item.listOfFile)
            {
                filesSize = filesSize + file.size;

            }
            ViewBag.size = filesSize;
            //set file icon
            if (item.listOfFile.Count == 1)
            {
                Models.File file = item.listOfFile.FirstOrDefault();
                string path = "~/Content/images/files_types/" + file.type.ToString() + ".png";
                if (System.IO.File.Exists(path))
                {
                    ViewBag.icon = file.type.ToString() + ".png";
                }
                else
                {
                    ViewBag.icon = "txt.png";
                }

            }
            else
            {
                Models.File file = item.listOfFile.FirstOrDefault();
                ViewBag.icon = "rar.png";
            }

            return View(item);
        }
        [Authorize]
        public ActionResult SetCookiesAndDownloadFile(int itemId)
        {
            String userId = User.Identity.GetUserId(); // get Current User
            string cookie = userId + itemId + "download";
            Item item = db.Items.Find(itemId);
            if (Request.Cookies[cookie] == null)
            {
                HttpCookie download = new HttpCookie(cookie);
                download[""] = cookie;
                download.Expires.AddMonths(2);
                Response.Cookies.Add(download);
                item.downloadNumber = item.downloadNumber + 1;
                if (ModelState.IsValid)
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                //cookie exist
            }
            return RedirectToAction("Download", new { itemId = itemId }); ;
        }
        [Authorize]
        //[download files]
        public ActionResult Download(int itemId)

        {
            //the file path stored in db like this ~/Files/file name.txt  
            Item item = db.Items.Find(itemId);
            if (item.listOfFile.Count == 1)
            {
                Models.File file = item.listOfFile.FirstOrDefault();
                string fileName = file.path;
                string contentType = file.type.ToString();
                var absolutePath = HttpContext.Server.MapPath("~/Content/Uploads/" + fileName);
                if (System.IO.File.Exists(absolutePath))
                {
                    TempData["successMessage"] = "Success";
                    return File(absolutePath, contentType, Path.GetFileName(fileName));
                }
                else
                {
                    TempData["errorMessage"] = "Not Found";
                    return RedirectToAction("File", new { fileId = item.Id });
                }

            }
            else if (item.listOfFile.Count == 0)
            {
                TempData["errorMessage"] = "This Item without Files";
                return RedirectToAction("File", new { fileId = item.Id });
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (Models.File file1 in item.listOfFile)
                        {
                            var absolutePath = HttpContext.Server.MapPath("~/Content/Uploads/" + file1.path);
                            if (System.IO.File.Exists(absolutePath))
                            {
                                TempData["successMessage"] = "Success";
                                zip.CreateEntry(file1.path, CompressionLevel.Fastest);
                            }
                            else
                            {
                                TempData["errorMessage"] = "One or more Of files are not Found";
                                return RedirectToAction("File", new { fileId = item.Id });
                            }

                        }
                    }
                    string fileName = item.title + ".zip";
                    string contentType = "application/zip";
                    return File(memoryStream.ToArray(), contentType, fileName);
                }
            }
        }
        [Authorize]
        public JsonResult AddLike(int itemId)
        {
            String userId = User.Identity.GetUserId();
            Favorite favorite = new Favorite();
            favorite.ItemId = itemId;
            favorite.UserId = userId;

            Favorite existFav = (from a in db.Favorites where a.UserId == userId && a.ItemId == itemId select a).FirstOrDefault();
            Item item = db.Items.Find(itemId);

            if (existFav == null)
            {
                item.likeNumber = item.likeNumber + 1;
                if (ModelState.IsValid)
                {
                    db.Favorites.Add(favorite);
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                item.likeNumber = item.likeNumber - 1;
                if (ModelState.IsValid)
                {
                    db.Favorites.Remove(existFav);
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json("Remove Like", JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize]
        public JsonResult Report(int itemId, string reportKey)
        {
            String userId = User.Identity.GetUserId();
            Report report = new Report();
            // get id for the selection Report
            Constant selection = (from a in db.Constants where a.Key == reportKey select a).FirstOrDefault();
            report.ConstantId = selection.Id;
            report.itemId = itemId;
            report.UserId = userId;
            Report existReport = (from a in db.Reports where a.UserId == userId && a.itemId == itemId select a).FirstOrDefault();
            if (existReport == null)
            {
                if (ModelState.IsValid)
                {
                    db.Reports.Add(report);
                    db.SaveChanges();
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Just One Report For Person", JsonRequestBehavior.AllowGet);
            }


        }



        public JsonResult getDepartment(String idString)
        {
            int i = 0;
            Int32.TryParse(idString, out i);
            db.Configuration.ProxyCreationEnabled = false;
            var deparments = db.Departments.Where(a => a.collegeId == i).ToList();
            return Json(deparments, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UniversityRequirements()
        {
            College UniversityRequierment = db.Colleges.Find(12);
            ViewBag.PageSize = defaultPageSize;
            Department requirement = UniversityRequierment.listOfDepartment.First();
            //define paging model
            Models.Paging paging = new Models.Paging();
            int offset = 1;
            int Page = 1;
            int Take = 10;
            if (Convert.ToInt32(Request.QueryString["Page"]) > 1)
            {
                Page = Convert.ToInt32(Request.QueryString["Page"]);
            }

            int skip = 0;
            if (Page == 1)
                skip = 0;
            else
                skip = ((Page - 1) * Take);
            int total = requirement.listOfCourse.Count();
            var data = requirement.listOfCourse.Skip(skip).Take(Take);
            string pagin = paging.Pagination(total, Page, Take, offset, "UniversityRequirements", "/UniversityRequirements", "");
            ViewBag.Paging = pagin;
            return View(data.ToList());
        }

    }
}