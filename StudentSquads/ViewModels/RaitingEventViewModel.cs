using StudentSquads.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentSquads.ViewModels
{
    public class RaitingEventViewModel
    {
       
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Введите наименование")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Уровень мероприятия")]
        public string  EventLevel { get; set; }
        [Display(Name = "Создано")]
        public string CreatedBy { get; set; }
        [Display(Name = "Утверждено")]
        public string Approved { get; set; }
        public string DateofBeginString { get; set; }
        public string DateofEndString { get; set; }
        [Display(Name = "Положение")]
        public string DocumentPath { get; set; }
        [Required(ErrorMessage = "Введите дату начала")]
        [Display(Name = "Дата начала")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofBegin { get; set; }
        [Required(ErrorMessage = "Введите дату окончания")]
        [Display(Name = "Дата окончания")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofEnd { get; set; }
        public List<EventLevel> EventLevels{ get; set; }
        [Required(ErrorMessage = "Введите уровень мероприятия")]
        [Display(Name = "Уровень мероприятия")]
        public int EventLevelId { get; set; }
    }
}