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
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Введите вашу фамилию")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Введите ваше имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Отчество")]
        public string PatronymicName { get; set; }
        //true - мужской, false-женский
        public bool Sex { get; set; }
        [Required(ErrorMessage = "Введите вашу дату рождения")]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofBirth { get; set; }
        [Display(Name = "Место обучения")]
        public string PlaceofStudy { get; set; }
        [Display(Name = "Форма обучения")]
        public string FormofStudy { get; set; }
        [Required(ErrorMessage = "Введите ИНН")]
        [Display(Name = "ИНН")]
        public string INN { get; set; }
        [Required(ErrorMessage = "Введите СНИЛС")]
        [Display(Name = "СНИЛС")]
        public string Snils { get; set; }
        [Required(ErrorMessage = "Введите номер телефона")]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Введите электронную почту")]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Введите серию паспорта")]
        [Display(Name = "Серия паспорта")]
        public string PassportSerie { get; set; }
        [Required(ErrorMessage = "Введите номер паспорта")]
        [Display(Name = "Номер паспорта")]
        public string PassportNumber{ get; set; }
        [Required(ErrorMessage = "Введите, кем выдан паспорт")]
        [Display(Name = "Кем выдан паспорт")]
        public string PassportGiven{ get; set; }
        [Required(ErrorMessage = "Введите дату выдачи паспорта")]
        [Display(Name = "Дата выдачи паспорта")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateofIssue  { get; set; }
        [Required(ErrorMessage = "Введите код подразделения")]
        [Display(Name = "Код подразделения")]
        public string DepartmentCode { get; set; }
        [Required(ErrorMessage = "Введите город рождения")]
        [Display(Name = "Город рождения")]
        public string CityofBirth { get; set; }
        [Required(ErrorMessage = "Введите место регистрации")]
        [Display(Name = "Место регистрации (по паспорту)")]
        public  string RegistrationPlace { get; set; }
        [Display(Name = "Дата вступления")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfEnter{ get; set; }
        [Display(Name = "Дата исключения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfExit { get; set; }
        public string ExitReason { get; set; }
        [Display(Name = "Номер членского билета")]
        public string MembershipNumber { get; set; }
        //null - зарегистрирован в системе,
        //false - из группы VK, true - сайт "Студотряды.рф"
        public bool? ApplicationFrom { get; set; }
        [Display(Name = "ФИО")]
        public string FIO { get; set; }
        [Required(ErrorMessage = "Введите ФИО в родительном падеже")]
        [Display(Name = "ФИО в родительном падеже")]
        public string FIOinGenetiv { get; set; }
        public string EnterDocumentPath { get; set; }
        public string ExitDocumentPath { get; set; }

    }
}