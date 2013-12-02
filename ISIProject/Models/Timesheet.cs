using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ISIProject.Models
{
    public enum TimesheetState
    {
        Open,
        Submitted,
        Aproved,
        Rejected
    }

    [Table("Timesheet")]
    public class Timesheet
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TimesheetId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EmployeeId { get; set; }
        public int ActivityId { get; set; }
        public bool ExtraHours { get; set; }
        public TimesheetState State { get; set;}
        public int? ProjectId { get; set; }
        public int? ClientId { get; set; }

        public virtual Activity Activity { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Project Project { get; set; }
        public virtual Client Client { get; set; }
    }
}