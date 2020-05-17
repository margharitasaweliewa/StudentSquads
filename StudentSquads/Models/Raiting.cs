using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.Models
{
    public class Raiting
    {
        public Guid Id { get; set; }
        public DateTime DateofBegin { get; set; }
        public DateTime? DateofEnd { get; set; }
        public DateTime? DateofCreation { get; set; }
        public string Comment{ get; set; }
    }
}