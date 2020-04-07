using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.ViewModels
{
    public class NewPersonViewModel
    {
        public IEnumerable<Squad> Squads { get; set; }
        public Person Person { get; set; }
        public Member Member { get; set; }
    }
}