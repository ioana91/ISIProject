using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DirectorManagerController : Controller
    {
        private CompanyContext db = new CompanyContext();

        //
        // GET: /DirectorManager/

        public ActionResult Index()
        {
            Employee director = new Employee();
            foreach (var employee in db.Employees)
            {
                if (Roles.IsUserInRole(employee.UserName, "Manager"))
                {
                    director = employee;
                }
            }
            return View(director);
        }

        //
        // GET: /DirectorManager/Details/5

        public ActionResult Details(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        [HttpGet]
        public ActionResult Select()
        {
            var employees = db.Employees.ToList();
            employees = employees.Where(e => e.UserName != "admin").ToList();

            Employee director = null;
            foreach (var employee in db.Employees)
            {
                if (Roles.IsUserInRole(employee.UserName, "Manager"))
                {
                    director = employee;
                }
            }

            if (director != null)
            {
                ViewBag.EmployeeId = new SelectList(employees, "EmployeeId", "Name", director.EmployeeId);
            }
            else
            {
                ViewBag.EmployeeId = new SelectList(employees, "EmployeeId", "Name", string.Empty);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Select(DirectorSelect selected)
        {
            Employee director = null;
            foreach (var employee in db.Employees)
            {
                if (Roles.IsUserInRole(employee.UserName, "Manager"))
                {
                    director = employee;
                }
            }

            if (director != null)
            {
                Roles.RemoveUserFromRole(director.UserName, "Manager");
                Roles.AddUserToRole(director.UserName, "Employee");
                director.IsRegular = true;
            }

            int selectedId = int.Parse(((string[])selected.SelectedValue)[0]);
            var newDirector = db.Employees.SingleOrDefault(e => e.EmployeeId == selectedId);
            newDirector.IsRegular = false;
            Roles.AddUserToRole(newDirector.UserName, "Manager");

            if (Roles.IsUserInRole(newDirector.UserName, "Employee"))
            {
                Roles.RemoveUserFromRole(newDirector.UserName, "Employee");
            }
            if (Roles.IsUserInRole(newDirector.UserName, "DivisionManager"))
            {
                Roles.RemoveUserFromRole(newDirector.UserName, "DivisionManager");
            }
            if (Roles.IsUserInRole(newDirector.UserName, "DepartmentManager"))
            {
                Roles.RemoveUserFromRole(newDirector.UserName, "DepartmentManager");
            }
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}