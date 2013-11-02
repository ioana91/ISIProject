﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ISIProject.Models
{
    [Table("Department")]
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int DivisionId { get; set; }
        [ForeignKey("DepartmentManagerId")]
        public int DepartmentManagerId { get; set; }

        public virtual Division Division { get; set; }
        public virtual Employee DepartmentManager { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}