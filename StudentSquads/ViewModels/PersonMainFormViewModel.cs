using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.ViewModels
{
    public class PersonMainFormViewModel
    {
        //Текущая личность
        public Person Person { get; set; }
        //Для отображения отрядов для выбора
        public IEnumerable<Squad> Squads { get; set; }
        //Выбранный отряд\
        [Display(Name = "Выберите отряд")]
        public Guid? SquadId { get; set; }
        //Для вывода списка штабов
        public IEnumerable<UniversityHeadquarter> UniversityHeadquarters { get; set; }
        [Display(Name = "Выберите штаб")]
        //Выбранный штаб
        public Guid? UniversityHeadquarterId { get; set; }
        //Для вывода информации о всех отряд, в которых была и есть личность
        public IEnumerable<Member> AllPersonSquads { get; set; }
        //Для отображения всех позиций, которые занимала или занимает личность
        public IEnumerable<HeadsOfStudentSquads> AllPersonPositions { get; set; }
        public  bool IsMember { get; set; }
        public  bool InOtherSquad { get; set; }
        public string file { get; set; }
    }
}