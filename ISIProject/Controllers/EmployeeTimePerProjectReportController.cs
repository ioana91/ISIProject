using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Department Manager")]
    public class EmployeeTimePerProjectReportController : Controller
    {
        private CompanyContext db = new CompanyContext();
        //
        // GET: /DepartmentReport1/

        public ActionResult Index()
        {
            var loggedUser = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            var employees = db.Employees.Where(e => e.DepartmentId == loggedUser.DepartmentId && 
                e.UserName != loggedUser.UserName);

            ViewBag.EmployeeId = new SelectList(employees, "EmployeeId", "Name", string.Empty);
            return View();
        }

        [HttpPost]
        public JsonResult SelectOptions(string selectedEmployee, DateTime startDate, DateTime endDate)
        {
            var employeeId = int.Parse(selectedEmployee);
            var employee = db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
            var timesheets = db.Timesheets.Where(t => t.EmployeeId == employee.EmployeeId && t.Activity.IsActive).ToList();

            var projects = timesheets.Select(t => t.Project.ProjectId).Distinct().ToList();
            
            string[][] data = new string[projects.Count() + 1][];
            for (int i = 0; i < projects.Count() + 1; i++)
            {
                data[i] = new string[2];
            }

            data[0][0] = "Employee Time per Project";
            data[0][1] = "Hours worked";

            var allProjects = db.Projects.ToList();
            for (int i = 0; i < projects.Count(); i++)
            {
                var timeWorked = timesheets.Where(t => t.ProjectId == projects[i] && t.StartTime.Date >= startDate 
                    && t.StartTime.Date <= endDate).
                    Sum(t => t.EndTime.TimeOfDay.TotalMinutes - t.StartTime.TimeOfDay.TotalMinutes);

                data[i + 1][0] = allProjects.FirstOrDefault(p => p.ProjectId == projects[i]).Client.Name + " - " +
                    allProjects.FirstOrDefault(p => p.ProjectId == projects[i]).Name;

                data[i + 1][1] = (timeWorked / 60).ToString();
            }

            return Json(data);
        }
    }
}
