using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Threading;
using ISIProject.Models;

namespace ISIProject
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            //Thread sendMailForUnsubmittedTimesheets = new Thread(MvcApplication.SendMailForUnsubmittedTimesheets);
            //sendMailForUnsubmittedTimesheets.IsBackground = true;
            //sendMailForUnsubmittedTimesheets.Start();

            //Application["SendMailForUnsubmittedTimesheets"] = sendMailForUnsubmittedTimesheets;
        }

        //protected void Application_End()
        //{
        //    try
        //    {
        //        Thread sendMailForUnsubmittedTimesheets = (Thread)Application["SendMailForUnsubmittedTimesheets"];
        //        if (sendMailForUnsubmittedTimesheets != null && sendMailForUnsubmittedTimesheets.IsAlive)
        //        {
        //            sendMailForUnsubmittedTimesheets.Abort();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //public static void SendMailForUnsubmittedTimesheets()
        //{
        //    while (true)
        //    {
        //        if (DateTime.Now.Day == 1)
        //        {
        //            CompanyContext db = new CompanyContext();
        //            var employees = db.Timesheets.Where(t => t.State == TimesheetState.Open || t.State == TimesheetState.Rejected).
        //                Select(t => t.EmployeeId).Distinct().ToList();

        //            foreach (var employeeId in employees)
        //            {
        //                var employee = db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
        //                var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
        //                {
        //                    Credentials = new System.Net.NetworkCredential("isiprojectri@gmail.com", "newP@ssword"),
        //                    EnableSsl = true
        //                };

        //                var from = "isiprojectri@gmail.com";
        //                var to = employee.Email;
        //                var subject = "Submit Timesheets";
        //                var body = "Please submit the timesheets for " +
        //                    System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1);
        //                client.Send(from, to, subject, body);
        //            }
        //        }

        //        Thread.Sleep(3600 * 1000);
        //    }
        //}
    }
}