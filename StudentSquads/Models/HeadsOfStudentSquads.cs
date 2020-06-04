using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.Models
{
    public class HeadsOfStudentSquads
    {
        //Надо подумать про добавление "Основной должности"
        public Guid Id { get; set; }
        [Display(Name = "Должность")]
        public string Position { get; set; }
        public MainPosition MainPosition { get; set; }
        [Display(Name = "Основная должность")]
        public int? MainPositionId { get; set; }
        public Squad Squad { get; set; }
        public Guid? SquadId { get; set; }
        public UniversityHeadquarter UniversityHeadquarter { get; set; }
        public Guid? UniversityHeadquarterId { get; set; }
        public RegionalHeadquarter RegionalHeadquarter { get; set; }
        public Guid? RegionalHeadquarterId { get; set; }
        public Person Person { get; set; }
        [Display(Name = "Член организации")]
        public Guid? PersonId { get; set; }
        [Display(Name = "Начало работы")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofBegin { get; set; }
        [Display(Name = "Окончание работы")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofEnd { get; set; }
        public bool HasRole { get; set; }

    }
}