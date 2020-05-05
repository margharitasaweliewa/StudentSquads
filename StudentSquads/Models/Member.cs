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
        [Display(Name = "Дата вступления")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfEnter { get; set; }
        [Display(Name = "Дата исключения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfExit { get; set; }
        [Display(Name = "Дата перехода в другой отряд")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime? DateOfTransition { get; set; }
        //Для отряда, из которого перешел член организации
        public Guid? FromSquadId { get; set; }
        public Squad FromSquad { get; set; }
        //Для отряда, в который переход член организации
        public Guid? ToSquadId { get; set; }
        public Squad ToSquad { get; set; }
        public bool? ApprovedByCommandStaff { get; set; }
        public string ExitReason { get; set; }
        //Ссылка на отряд
        public Squad Squad { get; set; }
        [Display(Name ="Отряд")]
        public Guid? SquadId { get; set; }
        //Ссылка на статус в отряде
        public Status Status { get; set; }
        public int? StatusId { get; set; }
        //Ссылка на личность
        public Person Person { get; set; }
        public Guid PersonId { get; set; }
        public string ApplicationStatus { get; set; }
        public string TransitionDocumentPath { get; set; }
    }
}