using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Department Manager")]
    public class WorkingHoursPerProjectReportController : Controller
    {
        private struct ProjectWorkStruct
        {
            public string project;
            public double workedTime;
        }

        private CompanyContext db = new CompanyContext();
        //
        // GET: /WorkingHoursPerProjectReport/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelectOptions(DateTime startDate, DateTime endDate)
        {
            var projects = selectProjects();
            var timesheets = db.Timesheets.ToList();

            string[][] data = new string[projects.Count() + 1][];
            for (int i = 0; i < projects.Count() + 1; i++)
            {
                data[i] = new string[2];
            }

            data[0][0] = "Working hours per Project";
            data[0][1] = "Hours worked";
                        
            for (int i = 0; i < projects.Count; i++)
            {
                var timeWorked = GetTimeWorked(startDate, endDate, projects, timesheets, i);

                data[i + 1][0] = projects[i].Client.Name + " - " + projects[i].Name;
                data[i + 1][1] = (timeWorked / 60).ToString();
            }

            return Json(data);
        }

        private static double GetTimeWorked(DateTime startDate, DateTime endDate, List<Project> projects, List<Timesheet> timesheets, int i)
        {
            return timesheets.Where(t => t.ProjectId == projects[i].ProjectId &&
                t.StartTime.Date >= startDate && t.StartTime.Date <= endDate).
                Sum(t => t.EndTime.TimeOfDay.TotalMinutes - t.StartTime.TimeOfDay.TotalMinutes);
        }

        private List<Project> selectProjects()
        {
            var loggedUser = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            var allProjects = db.Projects.ToList();
            var projects = allProjects.Where(p => p.Departments.Contains(loggedUser.Department)).ToList();
            return projects;
        }

        [HttpPost]
        public JsonResult OrderAscending(DateTime startDate, DateTime endDate)
        {
            var projects = selectProjects();
            var timesheets = db.Timesheets.ToList();

            string[][] data = new string[projects.Count() + 1][];
            for (int i = 0; i < projects.Count() + 1; i++)
            {
                data[i] = new string[2];
            }

            data[0][0] = "Working hours per Project";
            data[0][1] = "Hours worked";

            List<ProjectWorkStruct> projectSummary = new List<ProjectWorkStruct>();
            for (int i = 0; i < projects.Count; i++)
            {
                var timeWorked = GetTimeWorked(startDate, endDate, projects, timesheets, i);
                
                var summary = new ProjectWorkStruct();
                summary.workedTime = timeWorked;
                summary.project = projects[i].Client.Name + " - " + projects[i].Name;
                
                projectSummary.Add(summary);
            }

            var projectSummaryOrdered = projectSummary.OrderBy(p => p.workedTime).ToList();

            for (int i = 0; i < projectSummaryOrdered.Count; i++)
            {
                data[i + 1][0] = projectSummaryOrdered[i].project;
                data[i + 1][1] = (projectSummaryOrdered[i].workedTime / 60).ToString();
            }
            return Json(data);
        }

        [HttpPost]
        public JsonResult OrderDescending(DateTime startDate, DateTime endDate)
        {
            var projects = selectProjects();
            var timesheets = db.Timesheets.ToList();

            string[][] data = new string[projects.Count() + 1][];
            for (int i = 0; i < projects.Count() + 1; i++)
            {
                data[i] = new string[2];
            }

            data[0][0] = "Working hours per Project";
            data[0][1] = "Hours worked";

            List<ProjectWorkStruct> projectSummary = new List<ProjectWorkStruct>();
            for (int i = 0; i < projects.Count; i++)
            {
                var timeWorked = GetTimeWorked(startDate, endDate, projects, timesheets, i);

                var summary = new ProjectWorkStruct();
                summary.workedTime = timeWorked;
                summary.project = projects[i].Client.Name + " - " + projects[i].Name;

                projectSummary.Add(summary);
            }

            var projectSummaryOrdered = projectSummary.OrderByDescending(p => p.workedTime).ToList();

            for (int i = 0; i < projectSummaryOrdered.Count; i++)
            {
                data[i + 1][0] = projectSummaryOrdered[i].project;
                data[i + 1][1] = (projectSummaryOrdered[i].workedTime / 60).ToString();
            }
            return Json(data);
        }
    }
}
