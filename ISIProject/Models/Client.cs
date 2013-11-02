using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISIProject.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}