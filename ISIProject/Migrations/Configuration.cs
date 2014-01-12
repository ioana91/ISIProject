namespace ISIProject.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using ISIProject.Models;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<ISIProject.Models.CompanyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        CompanyContext db = new CompanyContext();

        protected override void Seed(ISIProject.Models.CompanyContext context)
        {
            WebSecurity.InitializeDatabaseConnection(
                "DefaultConnection",
                "Employee",
                "EmployeeId",
                "UserName", autoCreateTables: true);

            #region Roles
            if (!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");
            if (!Roles.RoleExists("Manager"))
                Roles.CreateRole("Manager");
            if (!Roles.RoleExists("Employee"))
                Roles.CreateRole("Employee");
            if (!Roles.RoleExists("Division Manager"))
                Roles.CreateRole("Division Manager");
            if (!Roles.RoleExists("Department Manager"))
                Roles.CreateRole("Department Manager");
            #endregion

            if (!WebSecurity.UserExists("rtapus"))
                WebSecurity.CreateUserAndAccount(
                    "rtapus",
                    "password",
                    new { Name = "Radu Tapus", Email = "radutzp@yahoo.com", IsRegular = true, IsAudited = false });
            if (!Roles.GetRolesForUser("rtapus").Contains("Employee"))
                Roles.AddUserToRole("rtapus", "Employee");

            if (!WebSecurity.UserExists("admin"))
                WebSecurity.CreateUserAndAccount(
                    "admin",
                    "password",
                    new { IsRegular = false, IsAudited = false });
            if (!Roles.GetRolesForUser("admin").Contains("Administrator"))
                Roles.AddUserToRole("admin", "Administrator");

            #region Activities
            Activity a1 = new Activity();
            a1.ActivityId = 1;
            a1.Name = "Vacation";
            a1.IsActive = false;
            if (!db.Activities.Any(a => a.Name == a1.Name))
            {
                db.Activities.Add(a1);
            }

            Activity a2 = new Activity();
            a2.ActivityId = 2;
            a2.Name = "Medical Leave";
            a2.IsActive = false;
            if (!db.Activities.Any(a => a.Name == a2.Name))
            {
                db.Activities.Add(a2);
            }

            Activity a3 = new Activity();
            a3.ActivityId = 3;
            a3.Name = "Legal Holidays";
            a3.IsActive = false;
            if (!db.Activities.Any(a => a.Name == a3.Name))
            {
                db.Activities.Add(a3);
            }

            Activity a4 = new Activity();
            a4.ActivityId = 4;
            a4.Name = "Developing";
            a4.IsActive = true;
            if (!db.Activities.Any(a => a.Name == a4.Name))
            {
                db.Activities.Add(a4);
            }

            Activity a5 = new Activity();
            a5.ActivityId = 5;
            a5.Name = "Testing";
            a5.IsActive = true;
            if (!db.Activities.Any(a => a.Name == a5.Name))
            {
                db.Activities.Add(a5);
            }

            Activity a6 = new Activity();
            a6.ActivityId = 6;
            a6.Name = "Learning";
            a6.IsActive = true;
            if (!db.Activities.Any(a => a.Name == a6.Name))
            {
                db.Activities.Add(a6);
            }

            db.SaveChanges();
            #endregion
        }
    }
}
