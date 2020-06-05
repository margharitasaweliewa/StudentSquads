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
    public class SquadsController : ApiController
    {
        private ApplicationDbContext _context;
        public SquadsController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        [HttpGet]
        public List<Squad> GetSquads(Guid Id)
        {
            List<Squad> listsquads = _context.Squads.Where(s => s.UniversityHeadquarterId == Id).ToList();
            return listsquads;
        }
    }
}
