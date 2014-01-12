using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISIProject.Models
{
    public class DepartmentReportModel
    {
        public string[] ReportCategories
        {
            get { return new string[] { "Employee Working Hours per Project", "Employees' Working Hours for Specified Project", "Working Hours per Project" }; }
        }

        public string SelectedReport { get; set; }
    }
}