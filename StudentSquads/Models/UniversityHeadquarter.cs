using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.Models
{
    public class UniversityHeadquarter
    {
        public Guid Id { get; set; }
        public string Name{ get; set; }
        public string University { get; set; }
        public string UniversityNumber{ get; set; }
        public RegionalHeadquarter RegionalHeadquarter { get; set; }
        public Guid? RegionalHeadquarterId { get; set; }

    }
}