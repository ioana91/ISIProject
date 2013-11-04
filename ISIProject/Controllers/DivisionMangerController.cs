using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    public class DivisionMangerController : Controller
    {
        private CompanyContext db = new CompanyContext();

        //
        // GET: /DivisionManger/

        public ActionResult Index()
        {
            var divisions = db.Divisions.Include(d => d.DivisionManager);
            return View(divisions.ToList());
        }

        //
        // GET: /DivisionManger/Details/5

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
        // GET: /DivisionManger/Create

        public ActionResult Create()
        {
            ViewBag.DivisionManagerId = new SelectList(db.Employees, "EmployeeId", "Name");
            return View();
        }

        //
        // POST: /DivisionManger/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Division division)
        {
            if (ModelState.IsValid)
            {
                db.Divisions.Add(division);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DivisionManagerId = new SelectList(db.Employees, "EmployeeId", "UserName", division.DivisionManagerId);
            return View(division);
        }

        //
        // GET: /DivisionManger/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Division division = db.Divisions.Find(id);
            if (division == null)
            {
                return HttpNotFound();
            }
            ViewBag.DivisionManagerId = new SelectList(db.Employees, "EmployeeId", "UserName", division.DivisionManagerId);
            return View(division);
        }

        //
        // POST: /DivisionManger/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Division division)
        {
            if (ModelState.IsValid)
            {
                db.Entry(division).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DivisionManagerId = new SelectList(db.Employees, "EmployeeId", "UserName", division.DivisionManagerId);
            return View(division);
        }

        //
        // GET: /DivisionManger/Delete/5

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
        // POST: /DivisionManger/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Division division = db.Divisions.Find(id);
            db.Divisions.Remove(division);
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