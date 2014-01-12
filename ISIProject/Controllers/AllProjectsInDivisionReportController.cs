using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;
using System.Web.Security;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Division Manager, Manager")]
    public class AllProjectsInDivisionReportController : Controller
    {
        private struct ProjectForDepartmentDetails
        {
            public int ProjectId;
            public int DepartmentId;
            public double Hours;
            public string ProjectName;
            public string DepartmentName;
        }

        private CompanyContext db = new CompanyContext();
        //
        // GET: /AllProjectsInDivision/

        public ActionResult Index()
        {
            if (Roles.IsUserInRole("Manager"))
            {
                ViewBag.DivisionId = new SelectList(db.Divisions, "DivisionId", "Name", string.Empty);
            }

            return View();
        }

        [HttpPost]
        public JsonResult SelectOptions(DateTime startDate, DateTime endDate, string selectedDepartment)
        {
            var allProjects = db.Projects.ToList();
            var allTimesheets = db.Timesheets.ToList();
            var loggedUser = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            Division division = new Division();

            if (Roles.IsUserInRole("Division Manager"))
            {
                division = db.Divisions.FirstOrDefault(d => d.DivisionManagerId == loggedUser.EmployeeId);
            }
            else
            {
                var divisionId = int.Parse(selectedDepartment);
                division = db.Divisions.FirstOrDefault(d => d.DivisionId == divisionId);
            }
            var departments = db.Departments.Where(d => d.DivisionId == division.DivisionId).ToList();

            var projects = new List<Project>();
            foreach (var department in departments)
            {
                var projectsInDepartment = allProjects.Where(p => p.Departments.Contains(department)).ToList();
                projects.AddRange(projectsInDepartment);
            }
            projects = projects.Distinct().ToList();
            var projectIds = projects.Select(p => p.ProjectId).ToList();

            var timesheets = allTimesheets.Where(t => t.ProjectId != null && projectIds.Contains((int)t.ProjectId) && t.StartTime.Date >= startDate
                && t.StartTime.Date <= endDate && departments.Contains(t.Employee.Department)).
                GroupBy(t => new {t.Employee.DepartmentId, t.ProjectId}).ToList();

            List<ProjectForDepartmentDetails> projectForDepartmentDetails = new List<ProjectForDepartmentDetails>();
            foreach (var timesheet in timesheets)
            {
                var detail = new ProjectForDepartmentDetails();
                detail.DepartmentId = (int)timesheet.Key.DepartmentId;
                detail.ProjectId = (int)timesheet.Key.ProjectId;
                detail.DepartmentName = db.Departments.FirstOrDefault(d => d.DepartmentId == detail.DepartmentId).Name;
                detail.ProjectName = allProjects.FirstOrDefault(p => p.ProjectId == detail.ProjectId).Client.Name + " - " 
                    + allProjects.FirstOrDefault(p => p.ProjectId == detail.ProjectId).Name;
                detail.Hours = timesheet.Sum(t => t.EndTime.TimeOfDay.TotalMinutes - t.StartTime.TimeOfDay.TotalMinutes);

                projectForDepartmentDetails.Add(detail);
            }

            var projectNames = projectForDepartmentDetails.Select(t => t.ProjectName).Distinct().ToList();
            var departmentNames = projectForDepartmentDetails.Select(t => t.DepartmentName).Distinct().ToList();

            string[][] data = new string[projectNames.Count() + 1][];
            for (int i = 0; i < projectNames.Count + 1; i++)
            {
                data[i] = new string[departmentNames.Count + 2];
            }

            data[0][0] = "Projects";
            for (int i = 0; i < departmentNames.Count; i++)
            {
                data[0][i + 1] = departmentNames[i];
            }
            data[0][departmentNames.Count + 1] = "Total";

            for (int i = 0; i < projectNames.Count; i++)
            {
                data[i + 1][0] = projectNames[i];
            }

            for (int i = 0; i < projectNames.Count; i++)
            {
                for (int j = 0; j < departmentNames.Count + 1; j++)
                {
                    data[i + 1][j + 1] = "0";
                }
            }

            for (int i = 0; i < projectNames.Count; i++)
            {
                for (int j = 0; j < departmentNames.Count + 1; j++)
                {
                    data[i + 1][j + 1] = (projectForDepartmentDetails.FirstOrDefault(d => d.DepartmentName == data[0][j + 1] &&
                        d.ProjectName == data[i + 1][0]).Hours / 60).ToString();
                }
            }

            for (int i = 0; i < projectNames.Count; i++)
            {
                for (int j = 0; j < departmentNames.Count; j++)
			    {
                    data[i + 1][departmentNames.Count + 1] = (double.Parse(data[i + 1][departmentNames.Count + 1]) +
                        double.Parse(data[i + 1][j + 1])).ToString();
			    }
            }

            return Json(data);
        }
    }
}
