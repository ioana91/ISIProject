using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ISIProject.Models
{
    public class CompanyContext : DbContext
    {
        public CompanyContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
    }
}