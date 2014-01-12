using ISIProject.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Manager,Department Manager,Division Manager")]
    public class DeptManagerTimesheetController : Controller
    {
        private CompanyContext db = new CompanyContext();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEmployees()
        {
            var manager = db.Employees.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (manager.DepartmentId != null)
            {
                return Json(db.Employees.Where(u => u.DepartmentId == manager.DepartmentId && u.EmployeeId != manager.EmployeeId).Select(u => new
                                {
                                    u.EmployeeId,
                                    u.Name
                                }).ToList());
            }
            if (Roles.IsUserInRole("Manager"))
            {
                return Json(db.Employees.Where(u => u.EmployeeId != manager.EmployeeId && u.UserName != "admin").Select(u => new
                {
                    u.EmployeeId,
                    u.Name
                }).ToList());
            }

            var division = db.Divisions.FirstOrDefault(d => d.DivisionManagerId == manager.EmployeeId);

            var depts = db.Departments.Where(d => d.DivisionId == division.DivisionId).Select(d => d.DepartmentId).ToList();

            var employees = db.Employees.Where(e => e.DepartmentId.HasValue ? depts.Contains(e.DepartmentId.Value) : false).Select(u => new
            {
                u.EmployeeId,
                u.Name
            }).ToList();

            return Json(employees);
        }

        public JsonResult GetEventsForEmployee(int employeeId)
        {
            var eventList = db.Timesheets.Where(t => t.EmployeeId == employeeId).ToList();

            var y = eventList.Select(tm =>
                        new
                        {
                            id = tm.TimesheetId,
                            start = tm.StartTime.ToString("MMMM dd, yyyy HH:mm:ss", new CultureInfo("en-US")),
                            end = tm.EndTime.ToString("MMMM dd, yyyy HH:mm:ss", new CultureInfo("en-US")),
                            title = (tm.Client != null ? tm.Client.Name + "<br/>" + tm.Project.Name + "<br/>" : "") + tm.Activity.Name,
                            body = tm.ProjectId.HasValue ? tm.Project.Name : string.Empty,
                            activityId = tm.ActivityId,
                            projectId = tm.ProjectId,
                            clientId = tm.ClientId,
                            state = (int)tm.State
                        }).ToArray();

            return Json(y);
        }

        [HttpPost]
        public void AproveTimesheet(int employeeId)
        {
            var employee = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            var employeeTimesheets = db.Timesheets.Where(t => t.EmployeeId == employeeId).ToList();
            var timesheetsToBeModified = employeeTimesheets.Where(t =>
               t.StartTime > new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1) &&
               t.EndTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59) &&
               (t.State == TimesheetState.Submitted || t.State == TimesheetState.Rejected)
               ).ToList();

            if (timesheetsToBeModified.Count() > 0)
            {
                timesheetsToBeModified.ForEach(t => t.State = TimesheetState.Aproved);

                db.SaveChanges();
            }

            if (employee.IsAudited)
            {
                var text = System.DateTime.Now + " " + Roles.GetRolesForUser()[0] + " " + employee.Name +
                    "has aproved the timesheets for " + CultureInfo.CurrentCulture.DateTimeFormat.
                    GetMonthName(DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1) + " for employee " +
                    db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId).Name + System.Environment.NewLine;

                LogAction(text);
            }
        }

        [HttpPost]
        public void RejectTimesheet(int employeeId, string message)
        {
            var employee = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            var employeeTimesheets = db.Timesheets.Where(t => t.EmployeeId == employeeId).ToList();
            var timesheetsToBeModified = employeeTimesheets.Where(t =>
               t.StartTime > new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1) &&
               t.EndTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59) &&
               t.State == TimesheetState.Submitted
               ).ToList();

            if (timesheetsToBeModified.Count() > 0)
            {
                timesheetsToBeModified.ForEach(t => t.State = TimesheetState.Rejected);

                db.SaveChanges();

                SendEmail(employeeId, message);
            }

            if (employee.IsAudited)
            {
                var text = System.DateTime.Now + " " + Roles.GetRolesForUser()[0] + " " + employee.Name +
                    "has rejected the timesheets for " + CultureInfo.CurrentCulture.DateTimeFormat.
                    GetMonthName(DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1) + " for employee " +
                    db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId).Name + System.Environment.NewLine;

                LogAction(text);
            }
        }

        private void SendEmail(int employeeId, string message)
        {
            var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential("isiprojectri@gmail.com", "newP@ssword"),
                EnableSsl = true
            };

            var employee = db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
            var from = "isiprojectri@gmail.com";
            var to = employee.Email;
            var subject = "Timesheet rejected";
            var body = message;
            client.Send(from, to, subject, body);
        }

        private void LogAction(string text)
        {
            System.IO.File.AppendAllText(@"C:\Users\Public\audit.txt", text);
        }
    }
}
