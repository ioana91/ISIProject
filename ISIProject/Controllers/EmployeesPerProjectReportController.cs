using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Department Manager")]
    public class EmployeesPerProjectReportController : Controller
    {
        private CompanyContext db = new CompanyContext();
        //
        // GET: /EmployeesPerProjectReport/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelectOptions(string searchString)
        {
            var loggedUser = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);

            var projects = db.Projects.ToList();
            var selectedProject = projects.FirstOrDefault(p => Regex.IsMatch(p.Name.ToLower(), searchString.ToLower()) && 
                p.Departments.Contains(loggedUser.Department));

            if (selectedProject != null)
            {
                var allEmployees = db.Employees.ToList();
                var timesheets = db.Timesheets.Where(t => t.ProjectId == selectedProject.ProjectId).ToList();
                var employees = timesheets.Select(t => t.EmployeeId).Distinct().ToList();
                
                string[][] data = new string[employees.Count() + 1][];
                for (int i = 0; i < employees.Count() + 1; i++)
                {
                    data[i] = new string[2];
                }

                data[0][0] = "Employees";
                data[0][1] = "Hours worked";

                for (int i = 0; i < employees.Count; i++)
                {
                    var timeWorked = timesheets.Where(t => t.ProjectId == selectedProject.ProjectId
                        && t.EmployeeId == employees[i]).
                        Sum(t => t.EndTime.TimeOfDay.TotalMinutes - t.StartTime.TimeOfDay.TotalMinutes);

                    data[i + 1][0] = allEmployees.FirstOrDefault(e => e.EmployeeId == employees[i]).Name;
                    data[i + 1][1] = (timeWorked / 60).ToString();
                }

                return Json(data);
            }
            else
            {
                return Json("error");
            }
        }
    }
}
