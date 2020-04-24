using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StudentSquads.Models;
using StudentSquads.ViewModels;

namespace StudentSquads.ViewModels
{
    public class ApplicationsListViewModel
    {
        public Member Member{ get; set; }
        public List<FeePayment> FeePayments { get; set; }
        public string Status { get; set; }
       

    }
}