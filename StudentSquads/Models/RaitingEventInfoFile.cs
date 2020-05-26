using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class RaitingEventInfoFile
    {
        public Guid Id { get; set; }
        public RaitingEventInfo RaitingEventInfo { get; set; }
        public Guid RaitingEventInfoId { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
    }
}