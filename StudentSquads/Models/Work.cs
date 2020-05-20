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
        public Guid Id { get; set; }
        public bool? Affirmed { get; set; }
        public string Rebuke { get; set; }
        public int? DUT { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofBegin { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofEnd { get; set; }
        public bool Alternative { get; set; }
        public string AlternativeReason { get; set; }
        public int? Season { get; set; }
        public WorkProject WorkProject { get; set; }
        public Guid? WorkProjectId { get; set; }
        public Member Member { get; set; }
        public Guid MemberId { get; set; }
        public Employer Employer { get; set; }
        public Guid EmployerId { get; set; }
        public DateTime CreateTime { get; set; }
        public Work OriginalWork { get; set; }
        public Guid? OriginalWorkId { get; set; }
        public bool? Approved { get; set; }
        public string ExitReason { get; set; }
        public WorkChangeType WorkChangeType { get; set; }
        public int? WorkChangeTypeId { get; set; }
        public DateTime? Removed { get; set; }
        //Вот тут вот по поводу новых записей
        //public bool? AddedToList { get; set; }
    }
}