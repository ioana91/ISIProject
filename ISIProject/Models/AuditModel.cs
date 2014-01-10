using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ISIProject.Models
{
    public class AuditModel
    {
        public static void Initialize()
        {
            db = new CompanyContext();

            //filter by role
            availableRoles = Roles.GetAllRoles();
            var roleList = availableRoles.ToList();
            roleList.Remove("Administrator");
            availableRoles = roleList.ToArray();
            availableRoles[0] = "Department Manager";
            availableRoles[1] = "Division Manager";
            
            selectedRoles = new List<bool>();
            for (int i = 0; i < availableRoles.Length; i++)
            {
                selectedRoles.Add(false);
            }

            //filter by divisions
            divisions = db.Divisions.ToList();
            selectedDivisions = new List<bool>();
            for (int i = 0; i < divisions.Count; i++)
            {
                selectedDivisions.Add(false);
            }

            //filter by department
            departments = db.Departments.ToList();
            selectedDepartments = new List<bool>();
            for (int i = 0; i < departments.Count; i++)
            {
                selectedDepartments.Add(false);
            }

            //filter by employees
            employees = db.Employees.ToList();
            employees = employees.Where(e => e.UserName != "admin").ToList();
            selectedEmployees = new List<bool>();
            employeeRoles = new List<string>();
            foreach (var employee in employees)
            {
                employeeRoles.Add(Roles.GetRolesForUser(employee.UserName)[0]);
                selectedEmployees.Add(false);
            }
        }

        static AuditModel()
        {
            if (availableRoles == null)
            {
                Initialize();
            }
        }

        public CompanyContext DB
        {
            get { return db; }
        }

        public string[] AvailableRoles
        {
            get { return availableRoles; }
            set { availableRoles = value; }
        }

        public List<bool> SelectedRoles
        {
            get { return selectedRoles; }
            set { selectedRoles = value; }
        }

        public List<Division> Divisions
        {
            get { return divisions; }
            set { divisions = value; }
        }

        public List<bool> SelectedDivisions
        {
            get { return selectedDivisions; }
            set { selectedDivisions = value; }
        }

        public List<Department> Departments
        {
            get { return departments; }
            set { departments = value; }
        }

        public List<bool> SelectedDepartments
        {
            get { return selectedDepartments; }
            set { selectedDepartments = value; }
        }

        public List<Employee> Employees
        {
            get { return employees; }
            set { employees = value; }
        }

        public List<bool> SelectedEmployees
        {
            get { return selectedEmployees; }
            set { selectedEmployees = value; }
        }

        public List<string> EmployeeRoles
        {
            get { return employeeRoles; }
            set { employeeRoles = value; }
        }

        private static CompanyContext db;
        private static string[] availableRoles;
        private static List<bool> selectedRoles;
        private static List<Division> divisions;
        private static List<bool> selectedDivisions;
        private static List<Department> departments;
        private static List<bool> selectedDepartments;
        private static List<Employee> employees;
        private static List<bool> selectedEmployees;
        private static List<string> employeeRoles;
    }
}