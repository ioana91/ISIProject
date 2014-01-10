using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;
using System.Globalization;
using System.Web.Script.Serialization;


namespace ISIProject.Controllers
{
    public class TimesheetController : Controller
    {
        //
        // GET: /Timesheet/
        private CompanyContext db = new CompanyContext();

        public ActionResult Index()
        {
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

            db.Timesheets.Add(timeSheet);
            db.SaveChanges();

            //var json = Json(new {newId = timeSheet.TimesheetId});

            return timeSheet.TimesheetId;
        }

        [HttpPost]
        public void UpdateEvent(CalendarEvent NewEvent)
        {
            var timeSheet = db.Timesheets.Where(tm => tm.TimesheetId == NewEvent.id).FirstOrDefault();

            timeSheet.StartTime = NewEvent.start;
            timeSheet.EndTime = NewEvent.end;
            timeSheet.Date = NewEvent.start.Date;
            timeSheet.ActivityId = Convert.ToInt32(NewEvent.activityId);

            if (!String.IsNullOrWhiteSpace(NewEvent.projectId))
            {
                timeSheet.ProjectId = Convert.ToInt32(NewEvent.projectId);
                timeSheet.ClientId = Convert.ToInt32(NewEvent.clientId);
            }
            timeSheet.ExtraHours = false;
            timeSheet.State = TimesheetState.Open;

            //db.Timesheets.Add(timeSheet);
            db.SaveChanges();
        }

        [HttpPost]
        public void RemoveEvent(CalendarEvent NewEvent)
        {
            var timeSheet = db.Timesheets.Where(tm => tm.TimesheetId == NewEvent.id).FirstOrDefault();
            db.Timesheets.Remove(timeSheet);
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
                }).OrderBy(a=>a.Name).ToArray();

            return Json(activities);
        }

        [HttpPost]
        public JsonResult GetClients()
        {
            var deptID = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name).DepartmentId;

            var clients = db.Clients.Where(c => c.Projects.Any(p => p.Departments.Any(d => d.DepartmentId == deptID))).Select(c=>new 
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
    }
}
