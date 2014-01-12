using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles="Manager")]
    public class ClientReportController : Controller
    {
        private CompanyContext db = new CompanyContext();
        //
        // GET: /ClientReport/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelectOptions(DateTime startDate, DateTime endDate)
        {
            var projects = db.Projects.OrderBy(p => p.ProjectId).ToList();

            string[][] data = new string[projects.Count + 1][];
            for (int i = 0; i < projects.Count + 1; i++)
            {
                data[i] = new string[2];
            }

            data[0][0] = "Clients";
            data[0][1] = "Hours worked";



            return Json(data);
        }
    }
}
