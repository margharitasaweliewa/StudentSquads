using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentSquads.ViewModels
{
    public class DesignationViewModel
    {
        public string FIO { get; set; }
        public string Position { get; set; }
        public  string DateofBegin { get; set; }
        public string DateofEnd { get; set; }
        public bool HasRole { get; set; }
        public string HasRoleString { get; set; }
        public Guid Id { get; set; }
    }
}