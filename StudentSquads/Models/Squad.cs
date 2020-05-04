using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class Squad
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofCreation { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateofLiquidation { get; set; }
        public UniversityHeadquarter UniversityHeadquarter { get; set; }
        public Guid? UniversityHeadquarterId { get; set; }
        public Direction Direction { get; set; }
        public int? DirectionId { get; set; }
    }
}