using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISIProject.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public int ClientId { get; set; }

        public virtual Client Client { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}