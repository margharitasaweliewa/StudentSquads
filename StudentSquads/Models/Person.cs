using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PatronymicName { get; set; }
        //true - мужской, false-женский
        public bool? Sex { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofBirth { get; set; }
        public string INN { get; set; }
        public string Snils { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PasportSerie { get; set; }
        public string PassportNumber{ get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofIssue  { get; set; }
        public string DepartmentCode { get; set; }
        public string CityofBirth { get; set; }
        public  string RegistrationPlace { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfEnter{ get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfExit { get; set; }
        public string MembershipNumber { get; set; }
        //null - зарегистрирован в системе,
        //false - из группы VK, true - сайт "Студотряды.рф"
        public bool? ApplicationFrom { get; set; }

    }
}