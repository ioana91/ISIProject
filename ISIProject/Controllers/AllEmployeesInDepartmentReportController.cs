using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles="Division Manager, Manager")]
    public class AllEmployeesInDepartmentReportController : Controller
    {
        CompanyContext db = new CompanyContext();
        //
        // GET: /AllEmployeesInDepartment/

        public ActionResult Index()
        {
            List<Department> departments = new List<Department>();

            var loggedUser = db.Employees.FirstOrDefault(e => e.UserName == User.Identity.Name);
            if (Roles.IsUserInRole(loggedUser.UserName, "Division Manager"))
            {
                departments = db.Departments.Where(d => d.Division.DivisionManagerId == loggedUser.EmployeeId).ToList();
            }
            else
            {
                departments = db.Departments.ToList();
            }

            ViewBag.DepartmentId = new SelectList(departments, "DepartmentId", "Name", string.Empty);
            return View();
        }

        [HttpPost]
        public JsonResult SelectOptions(string selectedDepartment)
        {
            var selectedDepartmentId = int.Parse(selectedDepartment);
            var employees = db.Employees.Where(e => e.DepartmentId == selectedDepartmentId).ToList();

            string[][] data = new string[employees.Count][];
            for (int i = 0; i < employees.Count; i++)
            {
                data[i] = new string[3];
            }

            for (int i = 0; i < employees.Count; i++)
            {
                data[i][0] = employees[i].Name;
                data[i][1] = employees[i].Email;
                data[i][2] = Roles.GetRolesForUser(employees[i].UserName)[0];
            }

            return Json(data);
        }
    }
}
