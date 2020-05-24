using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.ViewModels
{
    public class SquadWorkViewModel
    {
        public Guid Id { get; set; }
        public string Squad { get; set; }
        public string Uni { get; set; }
        public int Count { get; set; }
        public int AlternativeCount { get; set; }
        public int BudgesCount { get; set; }
        public string Affirmed { get; set; }
        public string Season { get; set; }

    }
}