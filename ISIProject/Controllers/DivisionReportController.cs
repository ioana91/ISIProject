﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Division Manager")]
    public class DivisionReportController : Controller
    {
        private CompanyContext db = new CompanyContext();
        //
        // GET: /DivisionReport/

        public ActionResult Index()
        {
            DivisionReportModel model = new DivisionReportModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DivisionReportModel model)
        {
            if (model.SelectedReport == "0")
            {
                return RedirectToAction("Index", "AllProjectsInDivisionReport");
            }
            else if (model.SelectedReport == "1")
            {
                return RedirectToAction("Index", "AllEmployeesInDepartmentReport");
            }

            return View(model);
        }
    }
}
