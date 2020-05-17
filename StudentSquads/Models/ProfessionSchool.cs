using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class ProfessionSchool
    {
        public Guid Id { get; set; }
        public Profession Profession { get; set; }
        public Guid ProfessionId { get; set; }
        public DateTime DateofBegin { get; set; }
        public DateTime DateofEnd { get; set; }
        public string Name { get; set; }
    }
}