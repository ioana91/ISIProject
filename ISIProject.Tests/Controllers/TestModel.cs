using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Data.Entity;
using ISIProject.Models;
using System.Linq;

namespace ISIProject.Tests.Controllers
{
    [TestClass]
    public class TestModel
    {
        CompanyContext mContext;

        [TestInitialize]
        public void SetUp()
        {
            mContext = new CompanyContext();
            
        }

        [TestMethod]
        public void AddEmployee_NewDataBase_EmployeeAdded()
        {           
            mContext.Employees.Add(new Employee
            {
                Name = "Radu Tapus"
            });

            mContext.SaveChanges();

            var employee = mContext.Employees.Where(a => a.Name == "Radu Tapus").FirstOrDefault();

            Assert.IsNotNull(employee);
        }
       
    }
}
