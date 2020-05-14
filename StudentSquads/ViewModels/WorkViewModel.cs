using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        [Display(Name = "Трудовой проект")]
        public string WorkProject { get; set; }
        [Display(Name = "Дата начала")]
        public DateTime DateofBegin { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTime DateofEnd { get; set; }
        [Display(Name = "Альтернативная целина")]
        public bool Alternative { get; set; }
        public string AlternativeString { get; set; }
        public string Affirmed { get; set; }
        public bool Choosen { get; set; }
        public List<Guid> PeopleId{ get; set; }
    }
}