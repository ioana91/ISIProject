using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ISIProject.Models;

namespace ISIProject.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AuditController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            AuditModel audit = new AuditModel();
            PopulateForm(audit);
            return View(audit);
        }

        [HttpPost]
        public ActionResult Index(AuditModel audit)
        {
            RemoveItems(audit);
            SaveAuditOptions(audit);
            AuditEmployees(audit);

            return View(audit);
        }

        private void AuditEmployees(AuditModel audit)
        {
            foreach (var item in audit.DB.AuditSelections)
            {
                if (item.EntityType == EntityType.Role)
                {
                    foreach (var employee in audit.Employees)
                    {
                        if (Roles.IsUserInRole(employee.UserName, item.EntityName))
                        {
                            employee.IsAudited = true;
                        }
                    }
                }

                if (item.EntityType == EntityType.Employee)
                {
                    foreach (var employee in audit.Employees)
                    {
                        if (employee.UserName == item.EntityName)
                        {
                            employee.IsAudited = true;
                        }
                    }
                }

                if (item.EntityType == EntityType.Department)
                {
                    foreach (var employee in audit.Employees)
                    {
                        if (employee.Department != null && employee.Department.Name == item.EntityName)
                        {
                            employee.IsAudited = true;
                        }
                    }
                }

                if (item.EntityType == EntityType.Division)
                {
                    var division = audit.Divisions.FirstOrDefault(d => d.Name == item.EntityName);

                    var departments = audit.Departments.Where(d => d.Division == division);
                    foreach (var department in departments)
                    {
                        foreach (var employee in audit.Employees)
                        {
                            if (employee.DepartmentId == department.DepartmentId)
                            {
                                employee.IsAudited = true;
                            }
                        }
                    }

                    division.DivisionManager.IsAudited = true;
                }
            }

            audit.DB.SaveChanges();
        }

        private void RemoveItems(AuditModel audit)
        {
            foreach (var item in audit.DB.AuditSelections)
            {
                audit.DB.AuditSelections.Remove(item);
            }
            audit.Employees.ForEach(e => e.IsAudited = false);
            audit.DB.SaveChanges();
        }

        private void SaveAuditOptions(AuditModel audit)
        {
            for (int i = 0; i < audit.SelectedRoles.Count; i++)
            {
                if (audit.SelectedRoles[i])
                {
                    audit.DB.AuditSelections.Add(new AuditSelections() 
                        { EntityType = EntityType.Role, EntityName = audit.AvailableRoles[i] });
                }
            }

            for (int i = 0; i < audit.SelectedDivisions.Count; i++)
            {
                if (audit.SelectedDivisions[i])
                {
                    audit.DB.AuditSelections.Add(new AuditSelections() 
                        { EntityType = EntityType.Division, EntityName = audit.Divisions[i].Name });
                }
            }

            for (int i = 0; i < audit.SelectedDepartments.Count; i++)
            {
                if (audit.SelectedDepartments[i])
                {
                    audit.DB.AuditSelections.Add(new AuditSelections() 
                        { EntityType = EntityType.Department, EntityName = audit.Departments[i].Name });
                }
            }

            for (int i = 0; i < audit.SelectedEmployees.Count; i++)
            {
                if (audit.SelectedEmployees[i])
                {
                    audit.DB.AuditSelections.Add(new AuditSelections() 
                        { EntityType = EntityType.Employee, EntityName = audit.Employees[i].UserName });
                }
            }

            audit.DB.SaveChanges();
        }

        private void PopulateForm(AuditModel audit)
        {
            foreach (var item in audit.DB.AuditSelections)
            {
                if (item.EntityType == EntityType.Role)
                {
                    for (int i = 0; i < audit.AvailableRoles.Length; i++)
                    {
                        if (audit.AvailableRoles[i] == item.EntityName)
                        {
                            audit.SelectedRoles[i] = true;
                        }
                    }
                }

                if (item.EntityType == EntityType.Division)
                {
                    for (int i = 0; i < audit.Divisions.Count; i++)
                    {
                        if (audit.Divisions[i].Name == item.EntityName)
                        {
                            audit.SelectedDivisions[i] = true;
                        }
                    }
                }

                if (item.EntityType == EntityType.Department)
                {
                    for (int i = 0; i < audit.Departments.Count; i++)
                    {
                        if (audit.Departments[i].Name == item.EntityName)
                        {
                            audit.SelectedDepartments[i] = true;
                        }
                    }
                }

                if (item.EntityType == EntityType.Employee)
                {
                    for (int i = 0; i < audit.Employees.Count; i++)
                    {
                        if (audit.Employees[i].UserName == item.EntityName)
                        {
                            audit.SelectedEmployees[i] = true;
                        }
                    }
                }
            }
        }
    }
}
