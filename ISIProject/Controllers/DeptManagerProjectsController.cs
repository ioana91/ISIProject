using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class DeptManagerProjectsController : Controller
    {
        //
        // GET: /DeptManagerProjects/

        public ActionResult Index()
        {
            DeptManagerSelectProjects selectProjects = new DeptManagerSelectProjects();
            var manager = selectProjects.DB.Employees.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var managerDepartment = selectProjects.DB.Departments.First(d => d.DepartmentId == manager.DepartmentId);
            for (int i = 0; i < selectProjects.Projects.Count; i++)
            {
                if (selectProjects.Projects[i].Departments.Contains(managerDepartment))
                {
                    selectProjects.IsSelected[i] = true;
                }
            }

            return View(selectProjects);
        }

        [HttpPost]
        public ActionResult Index(DeptManagerSelectProjects selectProjects)
        {
            var manager = selectProjects.DB.Employees.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var managerDepartment = selectProjects.DB.Departments.First(d => d.DepartmentId == manager.DepartmentId);
            for (int i = 0; i < selectProjects.IsSelected.Count; i++)
            {
                if (selectProjects.IsSelected[i])
                {
                    selectProjects.IsSelected[i] = false;
                    if (!selectProjects.Projects[i].Departments.Contains(managerDepartment))
                    {
                        selectProjects.Projects[i].Departments.Add(managerDepartment);
                    }
                }
                else
                {
                    if (selectProjects.Projects[i].Departments.Contains(managerDepartment))
                    {
                        selectProjects.Projects[i].Departments.Remove(managerDepartment);
                    }
                }
            }

            selectProjects.DB.SaveChanges();
            DeptManagerSelectProjects.Initialize();
            return View(selectProjects);
        }

    }
}
