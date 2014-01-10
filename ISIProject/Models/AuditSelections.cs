using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISIProject.Models
{
    public enum EntityType
    {
        Role,
        Division,
        Department,
        Employee
    }

    public class AuditSelections
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public EntityType EntityType { get; set; }
        public string EntityName { get; set; }
    }
}