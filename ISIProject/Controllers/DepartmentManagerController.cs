using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DepartmentManagerController : Controller
    {
        private CompanyContext db = new CompanyContext();

        //
        // GET: /DepartmentManager/

        public ActionResult Index()
        {
            var departments = db.Departments.Include(d => d.Division).Include(d => d.DepartmentManager);
            return View(departments.ToList());
        }

        //
        // GET: /DepartmentManager/Details/5

        public ActionResult Details(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // GET: /DepartmentManager/Create

        public ActionResult Create()
        {
            ViewBag.DivisionId = new SelectList(db.Divisions, "DivisionId", "Name");
            ViewBag.DepartmentManagerId = new SelectList(db.Employees, "EmployeeId", "Name")
                .Where(x => x.Text != string.Empty);
            return View();
        }

        //
        // POST: /DepartmentManager/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                if (!db.Departments.Any(d => d.Name.ToLower() == department.Name.ToLower()))
                {
                    db.Departments.Add(department);
                    var manager = db.Employees.SingleOrDefault(e => e.EmployeeId == department.DepartmentManagerId);
                    manager.DepartmentId = department.DepartmentId;
                    manager.IsRegular = false;
                    db.SaveChanges();

                    SetNewRole(manager);                   

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("UniqueName", "The name is not unique");
            }

            ViewBag.DivisionId = new SelectList(db.Divisions, "DivisionId", "Name", department.DivisionId);
            ViewBag.DepartmentManagerId = new SelectList(db.Employees, "EmployeeId", "Name", department.DepartmentManagerId);
            return View(department);
        }

        //
        // GET: /DepartmentManager/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            ViewBag.DivisionId = new SelectList(db.Divisions, "DivisionId", "Name", department.DivisionId);
            ViewBag.DepartmentManagerId = new SelectList(db.Employees, "EmployeeId", "Name", department.DepartmentManagerId)
                .Where(x => x.Text != string.Empty);
            return View(department);
        }

        //
        // POST: /DepartmentManager/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                var currentDepartment = db.Departments.Find(department.DepartmentId);
                var exManager = db.Employees.SingleOrDefault(e => e.EmployeeId == currentDepartment.DepartmentManagerId);
                exManager.DepartmentId = null;
                exManager.IsRegular = true;
                Roles.RemoveUserFromRole(exManager.UserName, "Department Manager");
                Roles.AddUserToRole(exManager.UserName, "Employee");

                db.Entry(currentDepartment).CurrentValues.SetValues(department);
                var manager = db.Employees.SingleOrDefault(e => e.EmployeeId == department.DepartmentManagerId);
                manager.DepartmentId = department.DepartmentId;
                manager.IsRegular = false;
                db.SaveChanges();

                SetNewRole(manager);

                return RedirectToAction("Index");
            }
            ViewBag.DivisionId = new SelectList(db.Divisions, "DivisionId", "Name", department.DivisionId);
            ViewBag.DepartmentManagerId = new SelectList(db.Employees, "EmployeeId", "Name", department.DepartmentManagerId);
            return View(department);
        }

        //
        // GET: /DepartmentManager/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // POST: /DepartmentManager/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);

            var manager = db.Employees.SingleOrDefault(e => e.EmployeeId == department.DepartmentManagerId);
            manager.IsRegular = true;
            manager.DepartmentId = null;
            db.SaveChanges();
            Roles.RemoveUserFromRole(manager.UserName, "Department Manager");
            Roles.AddUserToRole(manager.UserName, "Employee");

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void SetNewRole(Employee manager)
        {
            if (!Roles.IsUserInRole(manager.UserName, "Department Manager"))
            {
                Roles.AddUserToRole(manager.UserName, "Department Manager");
            }
            if (Roles.IsUserInRole(manager.UserName, "Employee"))
            {
                Roles.RemoveUserFromRole(manager.UserName, "Employee");
            }
            if (Roles.IsUserInRole(manager.UserName, "Division Manager"))
            {
                Roles.RemoveUserFromRole(manager.UserName, "Division Manager");
            }
            if (Roles.IsUserInRole(manager.UserName, "Manager"))
            {
                Roles.RemoveUserFromRole(manager.UserName, "Manager");
            }
        }
    }
}