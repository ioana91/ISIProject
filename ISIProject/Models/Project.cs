using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ISIProject.Models
{
    [Table("Project")]
    public class Project
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public int ClientId { get; set; }

        public virtual Client Client { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}