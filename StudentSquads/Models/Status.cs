using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class Status
    {
        [Display(Name = "Статус")]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}