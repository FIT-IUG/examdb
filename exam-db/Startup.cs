using exam_db.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(exam_db.Startup))]
namespace exam_db
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }

        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string defaultCollege = "Information Technology";
            string defaultDepartment = "Computer Science";
            var chkCollege = context.Colleges.FirstOrDefault(a => a.name.Equals(defaultCollege));
            if (chkCollege == null)
            {
                var college = new College();
                college.name = defaultCollege;
                context.Colleges.Add(college);
            }
            var chkDept = context.Departments.FirstOrDefault(a => a.name.Equals(defaultDepartment));
            if (chkDept == null)
            {
                var department = new Department();
                department.name = defaultDepartment;
                department.college = context.Colleges.FirstOrDefault(a => a.name.Equals(defaultCollege));
                context.Departments.Add(department);
            }
            context.SaveChanges();

            var user = new ApplicationUser();
            user.UserName = "omarskishko@gmail.com";
            user.Email = "omarskishko@gmail.com";
            user.firstname = "Omar";
            user.lastname = "Kishko";
            user.department = context.Departments.FirstOrDefault(a => a.name.Equals(defaultDepartment));
            string userPWD = "O@123456@kishko";

            var chkUser = UserManager.Create(user, userPWD);

        }
    }
}
