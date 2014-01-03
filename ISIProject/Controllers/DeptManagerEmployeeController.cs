using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    public class DeptManagerEmployeeController : Controller
    {
        private CompanyContext db = new CompanyContext();
        //
        // GET: /DeptManagerEmployee/

        public ActionResult Index()
        {
            return View(db.Employees.Where(e => e.UserName != "admin").
                Include("Department").ToList());
        }

    }
}
