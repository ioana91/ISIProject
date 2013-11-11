using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ISIProject.Models
{
    [Table("Division")]
    public class Division
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DivisionId { get; set; }
        public string Name { get; set; }
        public int DivisionManagerId { get; set; }

        [InverseProperty("EmployeeId")]
        [ForeignKey("DivisionManagerId")]
        public virtual Employee DivisionManager { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}