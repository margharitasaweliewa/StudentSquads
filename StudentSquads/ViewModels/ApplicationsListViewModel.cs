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
        public string FIO { get; set; }
        public string Sex { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string PlaceOfStudy { get; set; }
        public string Squad { get; set; }
        public string FeePayment { get; set; }
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string MembershipNumber { get; set; }
        public string OldSquad { get; set; }
        //Инфомармация о старом отряде (при подаче заявки на переход в другой отряд)
        public Guid OldId { get; set; }
        public string Status { get; set; }
        public bool Choosen { get; set; }
    }
}