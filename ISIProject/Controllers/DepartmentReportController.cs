using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Department Manager")]
    public class DepartmentReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            DepartmentReportModel reportModel = new DepartmentReportModel();
            return View(reportModel);
        }

        [HttpPost]
        public ActionResult Index(DepartmentReportModel reportModel)
        {
            if (reportModel.SelectedReport == "0")
            {
                return RedirectToAction("Index", "EmployeeTimePerProjectReport");
            }
            else if (reportModel.SelectedReport == "1")
            {
                return RedirectToAction("Index", "EmployeesPerProjectReport");
            }
            else if (reportModel.SelectedReport == "2")
            {
                return RedirectToAction("Index", "WorkingHoursPerProjectReport");
            }

            return View(reportModel);
        }
    }
}
