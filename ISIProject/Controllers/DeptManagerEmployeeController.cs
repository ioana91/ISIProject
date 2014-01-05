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
    public class DeptManagerEmployeeController : Controller
    {
        
        //
        // GET: /DeptManagerEmployee/

        [HttpGet]
        public ActionResult Index()
        {
            DeptManagerSelectEmp selectEmp = new DeptManagerSelectEmp();
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
                return Add(selectEmp);
            }
        }

        private ActionResult Search(DeptManagerSelectEmp selectEmp)
        {
            if (selectEmp.SearchString == string.Empty || selectEmp.SearchString == null)
            {
                return View(selectEmp);
            }

            if (selectEmp.SelectedCategory == "0")
            {
                selectEmp.Employees = selectEmp.Employees.Where(x => x.Name.ToLower().Contains(selectEmp.SearchString.ToLower())).ToList();
            }
            else if (selectEmp.SelectedCategory == "1")
            {
                selectEmp.Employees = selectEmp.Employees.Where(x => x.EmployeeId == int.Parse(selectEmp.SearchString)).ToList();
            }
            else
            {
                selectEmp.Employees = selectEmp.Employees.Where(x => x.Department.Name.ToLower() == selectEmp.SearchString.ToLower()).ToList();
            }

            return View(selectEmp);
        }

        private ActionResult Add(DeptManagerSelectEmp selectEmp)
        {
            for (int i = 0; i < selectEmp.IsSelected.Count; i++)
            {
                var a = selectEmp.IsSelected[i];
                try
                {
                    if (selectEmp.IsSelected[i])
                    {
                        selectEmp.IsSelected[i] = false;
                        selectEmp.Employees.First(e => e.EmployeeId == selectEmp.EmployeeIds[i]).DepartmentId = selectEmp.DB.Employees.First(x => x.UserName == User.Identity.Name).DepartmentId;
                        //selectEmp.Employees[selectEmp.EmployeeIds[i]].DepartmentId = selectEmp.DB.Employees.First(x => x.UserName == User.Identity.Name).DepartmentId;

                        selectEmp.DB.SaveChanges();
                    }
                }
                catch { }
            }
            return View(selectEmp);
        }

        private ActionResult Remove(DeptManagerSelectEmp selectEmp)
        {
            return View(selectEmp);
        }
    }
}
