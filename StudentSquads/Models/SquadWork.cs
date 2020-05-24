using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class SquadWork
    {
        public Guid Id{ get; set; }
        public int Season { get; set; }
        public bool Affirmed { get; set; }
        public int Count { get; set; }
        public int AlternativeCount { get; set; }
        public int BadgesCount { get; set; }
        public Squad Squad { get; set; }
        public Guid? SquadId { get; set; }
    }
}