using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class RaitingEventInfo
    {
        public Guid Id { get; set; }
        public Squad Squad { get; set; }
        public Guid? SquadId { get; set; }
        public RaitingSection RaitingSection { get; set; }
        public Guid RaitingSectionId { get; set; }
        public RaitingEvent RaitingEvent { get; set; }
        public Guid RaitingEventId { get; set; }
        public int MembershipCount { get; set; }
        public DateTime CreateTime { get; set; }
        public bool? Approved { get; set; }
    }
}