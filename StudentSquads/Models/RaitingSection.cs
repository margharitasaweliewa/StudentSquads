using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace StudentSquads.Models
{
    public class RaitingSection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CountTyoe { get; set; }
        public EventLevel EventLevel { get; set; }
        public Guid EventLevelId { get; set; }
        public MembershipType MembershipType { get; set; }
        public Guid MembershipTypeId { get; set; }
    }
}