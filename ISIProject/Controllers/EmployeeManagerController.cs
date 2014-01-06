using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    public class EmployeeManagerController : Controller
    {
        private CompanyContext db = new CompanyContext();

        //
        // GET: /EmployeeManager/

        public ActionResult Index()
        {
            return View(db.Employees.Where(e => e.UserName != "admin").
                Include("Department").ToList());
        }

        //
        // GET: /EmployeeManager/Details/5

        public ActionResult Details(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        //
        // GET: /EmployeeManager/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            return View();
        }

        //
        // POST: /EmployeeManager/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserName = model.FirstMidName.ToLower()[0] + model.LastName.ToLower();
                model.Name = model.FirstMidName + ' ' + model.LastName;

                var employee = new Employee()
                {
                    UserName = model.UserName,
                    DepartmentId = model.DepartmentId,
                    Email = model.Email,
                    Name = model.Name,
                    IsRegular = true
                };
                db.Employees.Add(employee);
                db.SaveChanges();

                WebSecurity.CreateAccount(model.UserName, model.Password);
                Roles.AddUserToRole(model.UserName, "Employee");
                
                return RedirectToAction("Index");
            }

            return View(model);
        }

        //
        // GET: /EmployeeManager/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            return View(employee);
        }

        //
        // POST: /EmployeeManager/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee employee)
        {
            String[] nameParts = employee.Name.Split(' ');
            employee.UserName = nameParts[0].ToLower()[0] + nameParts[nameParts.Length - 1].ToLower();

            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        //
        // GET: /EmployeeManager/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        //
        // POST: /EmployeeManager/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            Roles.RemoveUserFromRole(employee.UserName, "Employee");
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