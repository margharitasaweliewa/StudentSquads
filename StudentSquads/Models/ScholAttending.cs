using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class ScholAttending
    {
        public Guid Id{ get; set; }
        public Person Person { get; set; }
        public Guid PersonId { get; set; }
        public ProfessionSchool ProfessionSchool { get; set; }
        public Guid ProfessionSchoolId { get; set; }
        public string CertificateNumber { get; set; }
        public bool? Succeed { get; set; }
    }

}