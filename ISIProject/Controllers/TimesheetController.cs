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

            //return Json(new
            //{
            //    id = "1",
            //    start = new DateTime(2014, 1, 4, 14, 30, 0).ToString("MMMM dd, yyyy HH:mm:ss", new CultureInfo("en-US")),
            //    end = new DateTime(2014, 1, 4, 16, 0, 0).ToString("MMMM dd, yyyy HH:mm:ss", new CultureInfo("en-US")),
            //    title = "Super Test",
            //});
            var x = db.Timesheets.Where(tm => tm.EmployeeId == userID).ToList();

            var y = x.Select(tm =>
                        new
                        {
                            id = tm.TimesheetId,
                            start = tm.StartTime.ToString("MMMM dd, yyyy HH:mm:ss", new CultureInfo("en-US")),
                            end = tm.EndTime.ToString("MMMM dd, yyyy HH:mm:ss", new CultureInfo("en-US")),
                            title = tm.Activity.Name,
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
                ActivityId = 1,
                ExtraHours = false,
                State = TimesheetState.Open,
            };

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
            timeSheet.ActivityId = 1;
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
        public JsonResult GetActivities(string projectID)
        {
           // List<Activity> activities;

            if (projectID == "-1")
            {
                //activities = db.Activities.Select(new {})
            }
            else
            {

            }

            var activities = db.Activities.Select(a =>
                new
                {
                    a.ActivityId,
                    a.Name
                }).ToArray();


            return Json(activities);
        }
    }
}
