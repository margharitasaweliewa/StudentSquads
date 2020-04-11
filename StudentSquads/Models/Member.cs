using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.Models
{
    public class Member
    {
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofEnter { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofExit { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofTransition { get; set; }
        public bool ApprovedByCommandStaff { get; set; }
        public string ExitReason { get; set; }
        //Ссылка на отряд
        public Squad Squad { get; set; }
        public Guid? SquadId { get; set; }
        //Ссылка на статус в отряде
        public Status Status { get; set; }
        public int? StatusId { get; set; }
        //Ссылка на личность
        public Person Person { get; set; }
        public Guid PersonId { get; set; }

        // public Status Status { get; set; }
        //public int StatusId { get; set; }
    }
}