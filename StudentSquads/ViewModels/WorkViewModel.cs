using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.ViewModels
{
    public class WorkViewModel
    {
        public Guid PersonId { get; set; }
        public Guid Id { get; set; }
        [Display (Name ="ФИО")]
        public string FIO { get; set; }
        public  string Squad { get; set; }
        public string Uni{ get; set; }
        [Display(Name = "Работодатель")]
        public string Employer { get; set; }
        [Display(Name = "Работодатель")]
        public  Guid EmployerId { get; set; }
        [Display(Name = "Трудовой проект")]
        public string WorkProject { get; set; }
        public Guid? WorkProjectId { get; set; }
        [Display(Name = "Дата начала")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofBegin { get; set; }
        [Display(Name = "Дата окончания")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofEnd { get; set; }
        [Display(Name = "Альтернативная целина")]
        public bool Alternative { get; set; }
        public string AlternativeString { get; set; }
        public string Affirmed { get; set; }
        public bool Choosen { get; set; }
        public List<Guid> MembersIds { get; set; }
        public string DateofBeginString { get; set; }
        public string DateofEndString { get; set; }
        public bool Changed { get; set; }
        public List<WorkViewModel> Versions { get; set; }
        //Ведется ли аудит
        public bool Audit { get; set; }
        public string Season{ get; set; }
        public string CreateTime { get; set; }
        public Guid MemberId { get; set; }
        public string AlternativeReason { get; set; }
        public string ChangeType { get; set; }
        public string ApprovedString { get; set; }
    }
}