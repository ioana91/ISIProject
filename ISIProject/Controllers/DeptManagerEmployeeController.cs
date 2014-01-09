using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Web.Security;
using WebMatrix.WebData;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class DeptManagerEmployeeController : Controller
    {

        //
        // GET: /DeptManagerEmployee/

        private void AllowedEmployees(DeptManagerSelectEmp selectEmp)
        {
            selectEmp.Employees = selectEmp.Employees.Where(e => e.DepartmentId == null ||
                e.DepartmentId == selectEmp.DB.Employees.First(x => x.UserName == User.Identity.Name).DepartmentId).Where(e => e.IsRegular).ToList();
            selectEmp.EmployeeIds = selectEmp.EmployeeIds.Where
                (x => selectEmp.Employees.Select(e => e.EmployeeId).Contains(x)).ToList();

            for (int i = 0; i < selectEmp.Employees.Count; i++)
            {
                if (selectEmp.Employees[i].DepartmentId != null)
                {
                    selectEmp.IsSelected[i] = true;
                }
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            DeptManagerSelectEmp selectEmp = new DeptManagerSelectEmp();
            AllowedEmployees(selectEmp);
            return View(selectEmp);
        }

        [HttpPost]
        public ActionResult Index(DeptManagerSelectEmp selectEmp, string submitButton)
        {
            if (submitButton == "Search")
            {
                return Search(selectEmp);
            }
            else
            {
                return Save(selectEmp);
            }
        }

        private ActionResult Search(DeptManagerSelectEmp selectEmp)
        {
            if (selectEmp.SearchString == string.Empty || selectEmp.SearchString == null)
            {
                DeptManagerSelectEmp.Initialize();
                AllowedEmployees(selectEmp);
                return View(selectEmp);
            }

            if (selectEmp.SelectedCategory == "0")
            {
                selectEmp.Employees = selectEmp.Employees.Where
                    (x => x.Name.ToLower().Contains(selectEmp.SearchString.ToLower())).ToList();
            }
            else if (selectEmp.SelectedCategory == "1")
            {
                selectEmp.Employees = selectEmp.Employees.Where(x => x.EmployeeId == int.Parse(selectEmp.SearchString)).ToList();
            }
            else
            {
                selectEmp.Employees = selectEmp.Employees.Where(x => x.Department != null 
                    && x.Department.Name.ToLower() == selectEmp.SearchString.ToLower()).ToList();
            }

            selectEmp.EmployeeIds = selectEmp.EmployeeIds.Where
                (x => selectEmp.Employees.Select(e => e.EmployeeId).Contains(x)).ToList();
            return View(selectEmp);
        }

        private ActionResult Save(DeptManagerSelectEmp selectEmp)
        {
            var loggedUser = selectEmp.DB.Employees.First(x => x.UserName == User.Identity.Name);
            for (int i = 0; i < selectEmp.IsSelected.Count; i++)
            {
                var currentEmployee = selectEmp.Employees.First(e => e.EmployeeId == selectEmp.EmployeeIds[i]);
                if (selectEmp.IsSelected[i])
                {
                    selectEmp.IsSelected[i] = false;
                    currentEmployee.DepartmentId = loggedUser.DepartmentId;
                }
                else
                {
                    if (currentEmployee.DepartmentId ==
                        loggedUser.DepartmentId)
                    {
                        currentEmployee.DepartmentId = null;
                    }
                }
            }

            selectEmp.DB.SaveChanges();
            DeptManagerSelectEmp.Initialize();
            AllowedEmployees(selectEmp);
            return View(selectEmp);
        }
    }
}