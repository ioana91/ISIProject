using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISIProject.Controllers
{
    public class CalendarEvent
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string clientId { get; set; }
        public string productId { get; set; }
        public string activityId { get; set; }

    }
}
