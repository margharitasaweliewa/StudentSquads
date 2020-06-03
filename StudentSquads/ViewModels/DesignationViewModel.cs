using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing;
using StudentSquads.Models;

namespace StudentSquads.ViewModels
{
    public class DesignationViewModel
    {
        public string Place { get; set; }
        [Display(Name = "Протокол собрания")]
        public string DocumentPath { get; set; }

        [Display(Name = "Член организации")]
        public string FIO { get; set; }
        [Required(ErrorMessage = "Введите должность")]
        [Display (Name ="Должность")]
        public string Position { get; set; }
        [Display(Name = "Дата назначения")]
        public  string DateofBegin { get; set; }
        [Display(Name = "Дата отстранения")]
        public string DateofEnd { get; set; }
        public bool HasRole { get; set; }
        public string HasRoleString { get; set; }
        [Required(ErrorMessage = "Введите члена организации")]
        public Guid PersonId { get; set; }
        public List<Person> People { get; set; }
        public List<MainPosition> MainPositions { get; set; }
        [Display(Name = "Основная должность")]
        public int MainPositionId { get; set; }
        //ID в виде строки
        public string MainPosition { get; set; }
        public Guid HeadofStudentSquadsId{ get; set; }

    }
}