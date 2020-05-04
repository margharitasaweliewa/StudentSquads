using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class Direction
    {
        [Display(Name = "Направление")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}