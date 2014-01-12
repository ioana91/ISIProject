using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    public class ManagerReportController : Controller
    {
        //
        // GET: /ManagerReport/

        public ActionResult Index()
        {
            ManagerReportModel model = new ManagerReportModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ManagerReportModel model)
        {
            if (model.SelectedReport == "0")
            {
                return RedirectToAction("Index", "AllProjectsInDivisionReport");
            }
            else if (model.SelectedReport == "1")
            {
                return RedirectToAction("Index", "AllEmployeesInDepartmentReport");
            }
            else if (model.SelectedReport == "2")
            {
                return RedirectToAction("Index", "ClientReport");
            }

            return View(model);
        }
    }
}
