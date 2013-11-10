using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ISIProject.Models
{
    [Table("Employee")]
    public class Employee
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        //TODO: This should be modified when Department table contains data (OR NOT?)
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}