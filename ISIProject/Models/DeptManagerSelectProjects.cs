using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISIProject.Models
{
    public class DeptManagerSelectProjects
    {
        public static void Initialize()
        {
            db = new CompanyContext();
            projects = db.Projects.Include("Client").ToList();
            isSelected = new List<bool>();
            projectIds = new List<int>();
            foreach (var item in projects)
            {
                isSelected.Add(false);
                projectIds.Add(item.ProjectId);
            }
        }

        static DeptManagerSelectProjects()
        {
            if (db == null)
            {
                Initialize();
            }
        }

        public CompanyContext DB { get { return db; } }

        public List<Project> Projects
        {
            get { return projects; }
            set { projects = value; }
        }

        public List<bool> IsSelected
        {
            get { return isSelected; }
            set { isSelected = value;}
        }

        public List<int> ProjectIds
        {
            get { return projectIds; }
            set { projectIds = value; }
        }

        private static List<Project> projects;
        private static CompanyContext db;
        private static List<bool> isSelected;
        private static List<int> projectIds;
    }
}