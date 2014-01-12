using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISIProject.Models
{
    public class DivisionReportModel
    {
        public string[] ReportCategories
        {
            get { return new string[] { "All Projects in Division", "All Employees in Department"}; }
        }

        public string SelectedReport { get; set; }
    }
}