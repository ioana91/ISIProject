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
    }
}