using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ISIProject
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        double getNumberOfHoursWorked(int EmployeeId, DateTime startDate,DateTime EndDate);

        [OperationContract]
        double getNumberOfExtraHoursWorked(int EmployeeId, DateTime startDate, DateTime EndDate);

        [OperationContract]
        double getNumberOfVacationHours(int EmployeeId, DateTime startDate, DateTime EndDate);

        [OperationContract]
        double getNumberOfMedicalLeaveHours(int EmployeeId, DateTime startDate, DateTime EndDate);
    }
}
