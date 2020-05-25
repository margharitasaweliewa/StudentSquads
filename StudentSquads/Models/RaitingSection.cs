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
        public double Coef{ get; set; }
        public MembershipType MembershipType { get; set; }
        public Guid MembershipTypeId { get; set; }
        public bool Removed { get; set; }
    }
}