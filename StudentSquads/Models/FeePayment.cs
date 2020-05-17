using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StudentSquads.Models;

namespace StudentSquads.Models
{
    public class FeePayment
    {
        public Guid Id { get; set; }
        public DateTime DateofPayment{ get; set; }
        public int SumofPayment { get; set; }
        public Person Person { get; set; }
        public Guid PersonId { get; set; }
    }
}