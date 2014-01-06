using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;

namespace ISIProject.Models
{
    public class DeptManagerSelectEmp
    {
        public static void Initialize()
        {
            db = new CompanyContext();
            employees = db.Employees.Where(e => e.UserName != "admin").Include("Department").ToList();
            isSelected = new List<bool>();
            employeeIds = new List<int>();
            foreach (var item in employees)
            {
                isSelected.Add(false);
                employeeIds.Add(item.EmployeeId);
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