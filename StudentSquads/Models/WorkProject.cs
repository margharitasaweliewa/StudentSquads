using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class WorkProject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
        public bool Active { get; set; }

    }
}