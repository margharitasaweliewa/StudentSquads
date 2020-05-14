using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.Models
{
    public class Work
    {
        public Guid  Id { get; set; }
        public bool? Affirmed { get; set; }
        public string Rebuke { get; set; }
        public int? DUT { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateofBegin { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateofEnd { get; set; }
        public bool Alternative { get; set; }
        public string AlternativeReason{ get; set; }
        public string Season{ get; set; }
        public WorkProject WorkProject { get; set; }
        public Guid? WorkProjectId { get; set; }
        public Member Member { get; set; }
        public Guid MemberId { get; set; }
        public Employer Employer { get; set; }
        public Guid EmployerId { get; set; }
    }
}