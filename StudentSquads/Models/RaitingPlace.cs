using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class RaitingPlace
    {
        public Guid Id { get; set; }
        public Raiting Raiting { get; set; }
        public Guid RaitingId { get; set; }
        public RaitingSection RaitingSection { get; set; }
        public Guid RaitingSectionId { get; set; }
        public Squad Squad { get; set; }
        public Guid SquadId { get; set; }
        public int Place { get; set; }
        public int? MainPlace { get; set; }
    }
}