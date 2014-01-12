using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISIProject.Models
{
    public class ManagerReportModel
    {
        public string[] ReportCategories
        {
            get { return new string[] { "All Projects in Division", "All Employees in Department", "Client Report" }; }
        }

        public string SelectedReport { get; set; }
    }
}