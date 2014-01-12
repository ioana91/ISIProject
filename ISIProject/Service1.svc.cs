using ISIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ISIProject
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private CompanyContext db = new CompanyContext();

        public double getNumberOfHoursWorked(int EmployeeId, DateTime startDate, DateTime EndDate)
        {
            var list = db.Timesheets.Where(t => t.EmployeeId == EmployeeId
                                                       && t.StartTime > startDate
                                                       && t.EndTime < EndDate
                                                       && t.Activity.IsActive == true).ToList();
            double totalHours = list.Sum(t => t.EndTime.TimeOfDay.TotalMinutes - t.StartTime.TimeOfDay.TotalMinutes) / 60;
            double extraHours = list.Sum(t => t.ExtraHours);

            return totalHours - extraHours;
        }

        public double getNumberOfExtraHoursWorked(int EmployeeId, DateTime startDate, DateTime EndDate)
        {
            return db.Timesheets.Where(t => t.EmployeeId == EmployeeId
                                                       && t.StartTime > startDate
                                                       && t.EndTime < EndDate
                                                       && t.Activity.IsActive == true).Sum(t => t.ExtraHours);
        }

        public double getNumberOfVacationHours(int EmployeeId, DateTime startDate, DateTime EndDate)
        {
            var list = db.Timesheets.Where(t => t.EmployeeId == EmployeeId
                                                       && t.StartTime > startDate
                                                       && t.EndTime < EndDate
                                                       && t.ActivityId == 1).ToList();
            return list.Sum(t => t.EndTime.TimeOfDay.TotalMinutes - t.StartTime.TimeOfDay.TotalMinutes) / 60;
        }

        public double getNumberOfMedicalLeaveHours(int EmployeeId, DateTime startDate, DateTime EndDate)
        {
            var list = db.Timesheets.Where(t => t.EmployeeId == EmployeeId
                                                      && t.StartTime > startDate
                                                      && t.EndTime < EndDate
                                                      && t.ActivityId == 2).ToList();
            return list.Sum(t => t.EndTime.TimeOfDay.TotalMinutes - t.StartTime.TimeOfDay.TotalMinutes) / 60;
        }
    }
}
