using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Dynamic;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using StudentSquads.Controllers;

namespace StudentSquads.Controllers.API
{
    public class EmployersController : ApiController
    {
        private ApplicationDbContext _context;
        public EmployersController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        [HttpGet]
        public List<Employer> GetEmployers(string query = null)
        {
            //Нужно добавить функцию лимит
           List<Employer> employers = _context.Employers.ToList();
            if(query!=null)employers = employers.Where(e => e.Name.Contains(query)).ToList();
            return employers;
        }
        
    }
}
