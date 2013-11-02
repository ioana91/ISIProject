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

        protected override void Seed(ISIProject.Models.CompanyContext context)
        {
            WebSecurity.InitializeDatabaseConnection(
                "DefaultConnection",
                "Employee",
                "EmployeeId",
                "FirstMidName", autoCreateTables: true);

            if (!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");
            if (!Roles.RoleExists("Director"))
                Roles.CreateRole("Director");
            if (!Roles.RoleExists("Employee"))
                Roles.CreateRole("Employee");
            if (!Roles.RoleExists("DivisionManager"))
                Roles.CreateRole("DivisionManager");
            if (!Roles.RoleExists("DepartmentManager"))
                Roles.CreateRole("DepartmentManager");

            if (!WebSecurity.UserExists("lelong37"))
                WebSecurity.CreateUserAndAccount(
                    "lelong37",
                    "password",
                    new { FirstMidName = "A", LastName = "A" });

            if (!Roles.GetRolesForUser("lelong37").Contains("Administrator"))
                Roles.AddUsersToRoles(new[] { "lelong37" }, new[] { "Administrator" });

            if (!WebSecurity.UserExists("rtapus"))
                WebSecurity.CreateUserAndAccount(
                    "rtapus",
                    "password",
                    new { FirstMidName = "Radu", LastName = "Tapus" });
            if (!Roles.GetRolesForUser("rtapus").Contains("Employee"))
                Roles.AddUserToRole("rtapus", "Employee");

            //if (!WebSecurity.UserExists("lelong37"))
            //    WebSecurity.CreateUserAndAccount(
            //        "lelong37",
            //        "password",
            //        new { FirstName = "A"});

            //if (!Roles.GetRolesForUser("lelong37").Contains("Administrator"))
            //    Roles.AddUsersToRoles(new[] { "lelong37" }, new[] { "Administrator" });

            //if (!WebSecurity.UserExists("rtapus"))
            //    WebSecurity.CreateUserAndAccount(
            //        "rtapus",
            //        "password",
            //        new { FirstName = "Radu"});
            //if (!Roles.GetRolesForUser("rtapus").Contains("Employee"))
            //    Roles.AddUserToRole("rtapus", "Employee");
        }
    }
}
