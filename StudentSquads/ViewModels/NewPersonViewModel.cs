using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.ViewModels
{
    public class NewPersonViewModel
    {
        public IEnumerable<Squad> Squads { get; set; }
        public Person Person { get; set; }
        public Member Member { get; set; }
        public IEnumerable<MainPosition> MainPositions { get; set; }
        public HeadsOfStudentSquads HeadsOfStudentSquads { get; set; }
        public Guid Id { get; set; }
        public DateTime DateOfExit { get; set; }
        public string FIO { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string MembershipNumber { get; set; }
        public Guid SquadId { get; set; }
        public string SquadName { get; set; }
        public string StatusName { get; set; }
    }
}