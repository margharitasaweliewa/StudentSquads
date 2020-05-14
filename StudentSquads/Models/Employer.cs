using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class Employer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Director { get; set; }
        public string MainDocument { get; set; }
        public string OfficialName { get; set; }
        public string PhoneNumber{ get; set; }
        public string ContactPerson { get; set; }
    }
}