using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ISIProject.Models
{
    public class Division
    {
        public int DivisionId { get; set; }
        public string Name { get; set; }
        public int DivisionManagerId { get; set; }

        [ForeignKey("DivisionManagerId")]
        public virtual Employee DivisionManager { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}