using StudentSquads.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentSquads.ViewModels
{
    public class RaitingPlaceViewModel
    {
        public Guid? SquadId { get; set; }
        public string Squad { get; set; }
        public string Uni { get; set; }
        public string Raiting { get; set; }
        public int? MainPlace { get; set; }
        public int? Sectionplace { get; set; }
        public string Section { get; set; }
        public DateTime? RaitingDate { get; set; }
    }
}