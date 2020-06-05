using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.ViewModels
{
    public class RaitingSectionViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Введите наименование")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }
      
        [Display(Name = "Тип участия")]
        public string MembershipType{ get; set; }
        [Display(Name = "Уровень")]
        public string Level { get; set; }
        //Для уровней
        public List<int> LevelIds { get; set; }
        public List<RaitingSectionLevel> Levels { get; set; }
        [Required(ErrorMessage = "Введите тип участия")]
        [Display(Name = "Тип участия")]
        public int MembershipTypeId { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "Введите коэффициент")]
        [Display(Name = "Коэффициент")]
        public string Coef { get; set; }
        public List<MembershipType> MembershipTypes { get; set; }
        public string MembershipIdString { get; set; }
    }
}