using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.ViewModels
{
    public class RaitingEventInfoViewModel
    {
        public Guid Id { get; set; }
        public string Squad { get; set; }
        public string Uni { get; set; }
        [Display(Name = "Рейтинговое мероприятие")]
        public string Event { get; set; }
        public int FilesCount { get; set; }
        [Required(ErrorMessage = "Введите количество участников")]
        [Display(Name = "Количество участников")]
        public string MembershipCount { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
        public string RaitingSection { get; set; }
        public Guid EventId { get; set; }
        public string MembershipType { get; set; }
        [Required(ErrorMessage = "Введите тип участия")]
        [Display(Name = "Тип участия")]
        public string MembershipTypeId { get; set; }
        public List<MembershipType> MembershipTypes { get; set; }
        //Для добавления ссылка/описание
        public Dictionary<string, string> ReferenceDescriptions { get; set; }
    }
}