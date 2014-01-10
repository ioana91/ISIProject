using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Web.Security;
using WebMatrix.WebData;

namespace ISIProject.Models
{
    public class DeptManagerSelectEmp
    {
        public static void Initialize()
        {
            db = new CompanyContext();
            employees = db.Employees.Include("Department").ToList();
            isSelected = new List<bool>();
            employeeIds = new List<int>();
            foreach (var employee in employees)
            {
                isSelected.Add(false);
                employeeIds.Add(employee.EmployeeId);
            }
        }

        static DeptManagerSelectEmp()
        {
            if (db == null)
            {
                Initialize();
            }
        }

        public enum Categories
        {
            Name, Id, Department
        }

        public Categories[] SelectionCategories
        {
            get
            {
                return (Categories[])Enum.GetValues(typeof(Categories));
            }
        }

        public string SelectedCategory { get; set; }
        public string SearchString { get; set; }
        public CompanyContext DB { get { return db; } }

        public List<Employee> Employees
        {
            get { return employees; }
            set { employees = value; }
        }

        public List<bool> IsSelected
        {
            get { return isSelected; }
            set { isSelected = value;}
        }

        public List<int> EmployeeIds
        {
            get { return employeeIds; }
            set { employeeIds = value; }
        }

        private static List<Employee> employees;
        private static CompanyContext db;
        private static List<bool> isSelected;
        private static List<int> employeeIds;
    }
}