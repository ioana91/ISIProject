using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Division Manager, Manager")]
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

    }
}
