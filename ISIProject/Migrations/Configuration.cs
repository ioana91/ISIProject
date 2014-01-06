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
                "UserName", autoCreateTables: true);

            #region Roles
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
            #endregion

            if (!WebSecurity.UserExists("rtapus"))
                WebSecurity.CreateUserAndAccount(
                    "rtapus",
                    "password",
                    new { Name = "Radu Tapus", Email = "radutzp@yahoo.com", IsRegular = true });
            if (!Roles.GetRolesForUser("rtapus").Contains("Employee"))
                Roles.AddUserToRole("rtapus", "Employee");

            if (!WebSecurity.UserExists("admin"))
                WebSecurity.CreateUserAndAccount(
                    "admin",
                    "password",
                    new { IsRegular = false });
            if (!Roles.GetRolesForUser("admin").Contains("Administrator"))
                Roles.AddUserToRole("admin", "Administrator");
        }
    }
}
