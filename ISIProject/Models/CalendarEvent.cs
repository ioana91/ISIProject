using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISIProject.Models
{
    public class CalendarEvent
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string clientId { get; set; }
        public string projectId { get; set; }
        public string activityId { get; set; }
        public int state { get; set; }
    }
}