using exam_db.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace exam_db.Controllers
{
    public class WebController : Controller
    {
        private const int defaultPageSize =5 ;
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
        public ActionResult Getdept(int id , int pageIndex)
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
                Object myObject = new {
                    course = new Course() { Id = course.Id, name = course.name, code = course.code ,departmentId = course.departmentId }
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
            var list2 = JsonConvert.SerializeObject(Mycourses , Formatting.Indented);
            //string responseText = JSON.Validate(courses);
            //return Json( list , "application/json") ;
            return Json(new {
                data = list2,
                totalPage = totalPage
            }, JsonRequestBehavior.AllowGet);

        }
      
        public ActionResult Course(int courseId , String CollegeName)
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
            List<File> exams = new List<File>();
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
        public ActionResult GetFiles(String filetype,int courseId , int pageIndex)
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
                data = fileList ,
                totalPage = totalPage
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult File()
        {
            return View();
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