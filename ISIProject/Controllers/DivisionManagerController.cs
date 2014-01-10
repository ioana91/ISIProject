using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;
using System.Web.Security;

namespace ISIProject.Controllers
{
    [Authorize(Roles="Administrator")]
    public class DivisionManagerController : Controller
    {
        private CompanyContext db = new CompanyContext();

        //
        // GET: /DivisionManager/

        public ActionResult Index()
        {
            var divisions = db.Divisions.Include(d => d.DivisionManager);
            return View(divisions.ToList());
        }

        //
        // GET: /DivisionManager/Details/5

        public ActionResult Details(int id = 0)
        {
            Division division = db.Divisions.Find(id);
            if (division == null)
            {
                return HttpNotFound();
            }
            return View(division);
        }

        //
        // GET: /DivisionManager/Create

        public ActionResult Create()
        {
            ViewBag.DivisionManagerId = new SelectList(db.Employees, "EmployeeId", "Name")
                .Where(x => x.Text != string.Empty);
            return View();
        }

        //
        // POST: /DivisionManager/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Division division)
        {
            if (ModelState.IsValid)
            {
                if (!db.Divisions.Any(d => d.Name.ToLower() == division.Name.ToLower()))
                {
                    db.Divisions.Add(division);
                    var manager = db.Employees.SingleOrDefault(e => e.EmployeeId == division.DivisionManagerId);
                    manager.IsRegular = false;
                    db.SaveChanges();

                    SetNewRole(manager);

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("UniqueName", "The name is not unique");
            }
            ViewBag.DivisionManagerId = new SelectList(db.Employees, "EmployeeId", "Name", division.DivisionManagerId);
            return View(division);

        }

        //
        // GET: /DivisionManager/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Division division = db.Divisions.Find(id);
            if (division == null)
            {
                return HttpNotFound();
            }

            ViewBag.DivisionManagerId = new SelectList(db.Employees, "EmployeeId", "Name", division.DivisionManagerId)
                .Where(x => x.Text != string.Empty);
            return View(division);
        }

        //
        // POST: /DivisionManager/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Division division)
        {
            if (ModelState.IsValid)
            {
                var currentDivision = db.Divisions.Find(division.DivisionId);
                var exManager = db.Employees.SingleOrDefault(e => e.EmployeeId == currentDivision.DivisionManagerId);
                exManager.IsRegular = true;
                Roles.RemoveUserFromRole(exManager.UserName, "Division Manager");
                Roles.AddUserToRole(exManager.UserName, "Employee");

                db.Entry(currentDivision).CurrentValues.SetValues(division);
                var manager = db.Employees.SingleOrDefault(e => e.EmployeeId == division.DivisionManagerId);
                manager.IsRegular = false;
                db.SaveChanges();

                SetNewRole(manager);

                return RedirectToAction("Index");
            }
            ViewBag.DivisionManagerId = new SelectList(db.Employees, "EmployeeId", "Name", division.DivisionManagerId);
            return View(division);
        }

        //
        // GET: /DivisionManager/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Division division = db.Divisions.Find(id);
            if (division == null)
            {
                return HttpNotFound();
            }
            return View(division);
        }

        //
        // POST: /DivisionManager/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Division division = db.Divisions.Find(id);
            db.Divisions.Remove(division);
            
            var manager = db.Employees.SingleOrDefault(e => e.EmployeeId == division.DivisionManagerId);
            manager.IsRegular = true;
            Roles.RemoveUserFromRole(manager.UserName, "Division Manager");
            Roles.AddUserToRole(manager.UserName, "Employee");
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void SetNewRole(Employee manager)
        {
            if (!Roles.IsUserInRole(manager.UserName, "Division Manager"))
            {
                Roles.AddUserToRole(manager.UserName, "Division Manager");
            }
            if (Roles.IsUserInRole(manager.UserName, "Employee"))
            {
                Roles.RemoveUserFromRole(manager.UserName, "Employee");
            }
            if (Roles.IsUserInRole(manager.UserName, "Manager"))
            {
                Roles.RemoveUserFromRole(manager.UserName, "Manager");
            }
            if (Roles.IsUserInRole(manager.UserName, "Department Manager"))
            {
                Roles.RemoveUserFromRole(manager.UserName, "Department Manager");
            }
        }
    }
}