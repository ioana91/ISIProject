using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.IO;
using System.Runtime.Serialization;
using System.Data.Objects.DataClasses;
using System.Reflection;
using System.Data.Objects;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Manager, Employee, Division Manager, Department Manager")]
    public class TimesheetController : Controller
    {
        //
        // GET: /Timesheet/
        private CompanyContext db = new CompanyContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexSubmit()
        {
            var text = string.Empty;

            SendEmail();
            ChangeTimesheetState();

            var employee = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            if (employee.IsAudited)
            {
                text += DateTime.Now + " Employee " + employee.Name + " has submitted the timesheets for " +
                    CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName
                    (DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1) + System.Environment.NewLine;
                LogAction(text);
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetEvents()
        {
            var loggedInUserName = User.Identity.Name;
            int userID = db.Employees.Where(e => e.UserName == loggedInUserName).FirstOrDefault().EmployeeId;

            var x = db.Timesheets.Where(tm => tm.EmployeeId == userID).ToList();

            var y = x.Select(tm =>
                        new
                        {
                            id = tm.TimesheetId,
                            start = tm.StartTime.ToString("MMMM dd, yyyy HH:mm:ss", new CultureInfo("en-US")),
                            end = tm.EndTime.ToString("MMMM dd, yyyy HH:mm:ss", new CultureInfo("en-US")),
                            title = tm.Activity.Name,
                            body = tm.ProjectId.HasValue ? tm.Project.Name : string.Empty,
                            activityId = tm.ActivityId,
                            projectId = tm.ProjectId,
                            clientId = tm.ClientId,
                            state = (int)tm.State
                        }).ToArray();

            var json = Json(y);

            return json;
        }

        [HttpPost]
        public int AddEvent(CalendarEvent NewEvent)
        {
            
            var timeSheet = new Timesheet
            {
                StartTime = NewEvent.start,
                EndTime = NewEvent.end,
                Date = NewEvent.start.Date,
                EmployeeId = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).EmployeeId,
                ActivityId = Convert.ToInt32(NewEvent.activityId),
                ExtraHours = false,
                State = TimesheetState.Open,
            };

            if (!String.IsNullOrWhiteSpace(NewEvent.projectId))
            {
                timeSheet.ProjectId = Convert.ToInt32(NewEvent.projectId);
                timeSheet.ClientId = Convert.ToInt32(NewEvent.clientId);
            }

            var text = logForAddEvent(timeSheet);

            db.Timesheets.Add(timeSheet);
            db.SaveChanges();

            if (db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).IsAudited)
            {
                LogAction(text);
            }

            return timeSheet.TimesheetId;
        }

        private string logForAddEvent(Timesheet timeSheet)
        {
            var text = string.Empty;
            text += System.DateTime.Now + " " + Roles.GetRolesForUser(User.Identity.Name)[0] + " " +
                db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).Name + " added a new timesheet entry for date " +
                    timeSheet.StartTime.ToString("D") + " StartTime: " + timeSheet.StartTime.TimeOfDay + " EndTime: " + timeSheet.EndTime.TimeOfDay;
            if (db.Activities.FirstOrDefault(a => a.ActivityId == timeSheet.ActivityId).IsActive)
            {
                text += " Client: " + db.Clients.FirstOrDefault(c => c.ClientId == timeSheet.ClientId).Name +
                    " Project: " + db.Projects.FirstOrDefault(p => p.ProjectId == timeSheet.ProjectId).Name;
            }
            text += " Activity: " + db.Activities.FirstOrDefault(a => a.ActivityId == timeSheet.ActivityId).Name +
                System.Environment.NewLine;
            return text;
        }

        [HttpPost]
        public void UpdateEvent(CalendarEvent NewEvent)
        {
 

            var timeSheet = db.Timesheets.Where(tm => tm.TimesheetId == NewEvent.id).FirstOrDefault();

            var text = string.Empty;
            text += System.DateTime.Now + " " + Roles.GetRolesForUser(User.Identity.Name)[0] + " " +
                db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).Name + " updated timesheet entry for date " +
                timeSheet.StartTime.ToString("D");

            if (timeSheet.StartTime != NewEvent.start)
            {
                text += " StartTime modified (" + timeSheet.StartTime.TimeOfDay + "-" + NewEvent.start.TimeOfDay + ")";
                timeSheet.StartTime = NewEvent.start;
            }
            if (timeSheet.EndTime != NewEvent.end)
            {
                text += " EndTime modified (" + timeSheet.EndTime.TimeOfDay + "-" + NewEvent.end.TimeOfDay + ")";
                timeSheet.EndTime = NewEvent.end;
            }
            if (timeSheet.Date != NewEvent.start.Date)
            {
                text += " Date modified (" + timeSheet.Date.ToString("D") + "-" + NewEvent.start.Date.ToString("D") + ")";
                timeSheet.Date = NewEvent.start.Date;
            }
            if (timeSheet.ActivityId != Convert.ToInt32(NewEvent.activityId))
            {
                var activityId = Convert.ToInt32(NewEvent.activityId);
                var isActive = db.Activities.Where(a => a.ActivityId == activityId).Select(a => a.IsActive).FirstOrDefault();

                if (!isActive)
                {
                    timeSheet.ProjectId = null;
                    timeSheet.ClientId = null;
                }

                text += " Activity modified (" + db.Activities.FirstOrDefault(a => a.ActivityId == timeSheet.ActivityId).Name +
                    "-" + db.Activities.FirstOrDefault(a => a.ActivityId == activityId).Name + ")";
                timeSheet.ActivityId = activityId;
            }
            if (!String.IsNullOrWhiteSpace(NewEvent.projectId))
            {
                if (timeSheet.ProjectId != Convert.ToInt32(NewEvent.projectId))
                {
                    var projectId = Convert.ToInt32(NewEvent.projectId);
                    text += " Project modified (" + db.Projects.FirstOrDefault(p => p.ProjectId == timeSheet.ProjectId).Name +
                        " from client " + db.Projects.FirstOrDefault(p => p.ProjectId == timeSheet.ProjectId).Client.Name + "-" +
                        db.Projects.FirstOrDefault(p => p.ProjectId == projectId).Name + " from client " +
                        db.Projects.FirstOrDefault(p => p.ProjectId == projectId).Client.Name + ")";

                    timeSheet.ProjectId = projectId;
                }
                timeSheet.ClientId = Convert.ToInt32(NewEvent.clientId);
            }
            timeSheet.ExtraHours = false;
            timeSheet.State = (TimesheetState)NewEvent.state;

            text += System.Environment.NewLine;
            if (db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).IsAudited)
            {
                LogAction(text);
            }

            //db.Timesheets.Add(timeSheet);
            db.SaveChanges();
        }

        [HttpPost]
        public void RemoveEvent(CalendarEvent NewEvent)
        {
            var text = string.Empty;
            var timeSheet = db.Timesheets.Where(tm => tm.TimesheetId == NewEvent.id).FirstOrDefault();

            text += System.DateTime.Now + " " + Roles.GetRolesForUser(User.Identity.Name)[0] + " " +
                db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).Name + " removed timesheet entry for date " +
                timeSheet.StartTime.ToString("D") + " StartTime: " + timeSheet.StartTime.TimeOfDay + " EndTime: " + timeSheet.EndTime.TimeOfDay;
            if (db.Activities.FirstOrDefault(a => a.ActivityId == timeSheet.ActivityId).IsActive)
            {
                text += " Client: " + db.Clients.FirstOrDefault(c => c.ClientId == timeSheet.ClientId).Name +
                    " Project: " + db.Projects.FirstOrDefault(p => p.ProjectId == timeSheet.ProjectId).Name;
            }
            text += " Activity: " + db.Activities.FirstOrDefault(a => a.ActivityId == timeSheet.ActivityId).Name +
                System.Environment.NewLine;

            db.Timesheets.Remove(timeSheet);

            if (db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).IsAudited)
            {
                LogAction(text);
            }

            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult GetActivities()
        {
            var activities = db.Activities.Select(a =>
                new
                {
                    a.ActivityId,
                    a.Name,
                    a.IsActive
                }).OrderBy(a => a.Name).ToArray();

            return Json(activities);
        }

        [HttpPost]
        public JsonResult GetClients()
        {
            var deptID = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).DepartmentId;

            var clients = db.Clients.Where(c => c.Projects.Any(p => p.Departments.Any(d => d.DepartmentId == deptID))).Select(c => new
                {
                    c.ClientId,
                    c.Name
                }).OrderBy(c => c.Name).ToArray();

            return Json(clients);
        }

        [HttpPost]
        public JsonResult GetProjects(string clientId)
        {
            int ClientID = Convert.ToInt32(clientId);
            var projects = db.Projects.Where(p => p.ClientId == ClientID).Select(p =>
                new
                {
                    p.ProjectId,
                    p.Name
                }).OrderBy(p => p.Name).ToArray();

            return Json(projects);
        }

        [HttpPost]
        public void DuplicateEvents(DateTime date)
        {
            try
            {
                var timeSheets = db.Timesheets.AsNoTracking().Where(t => EntityFunctions.TruncateTime(t.Date) == date).ToList();
                timeSheets.ForEach(t =>
                {
                    t.Date = DateTime.Now.Date;
                    TimeSpan ts = new TimeSpan(t.StartTime.Hour,t.StartTime.Minute,t.StartTime.Second);
                    t.StartTime= DateTime.Now.Date + ts;
                    ts = new TimeSpan(t.EndTime.Hour, t.EndTime.Minute, t.EndTime.Second);
                    t.EndTime = DateTime.Now.Date + ts;
                    db.Timesheets.Add(t);
                });

                db.SaveChanges();
            }
            catch (Exception e)
            {
                e.Equals(new Exception());
            }
        }

        private void LogAction(string text)
        {
            System.IO.File.AppendAllText(@"C:\Users\Public\audit.txt", text);
        }

        private void SendEmail()
        {
            var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential("isiprojectri@gmail.com", "newP@ssword"),
                EnableSsl = true
            };

            var employee = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            var from = "isiprojectri@gmail.com";
            var to = employee.Department.DepartmentManager.Email;
            var subject = "Sent Timesheet";
            var body = "Employee " + employee.Name + " has sent you the timesheets for " +
                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1);
            client.Send(from, to, subject, body);
        }

        private void ChangeTimesheetState()
        {
            var employee = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            var month = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
            db.Timesheets.Where(t => t.EmployeeId == employee.EmployeeId && t.Date.Month == month).ToList().
                ForEach(t => t.State = TimesheetState.Submitted);
            db.SaveChanges();
        }
    }
}
