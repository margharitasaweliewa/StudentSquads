using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class RaitingSectionLevel
    {
        public Guid Id { get; set; }
        public RaitingSection RaitingSection { get; set; }
        public Guid RaitingSectionId { get; set; }
        public EventLevel EventLevel { get; set; }
        public int EventLevelId { get; set; }
    }
}