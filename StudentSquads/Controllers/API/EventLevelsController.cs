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
    public class EventLevelsController : ApiController
    {
        private ApplicationDbContext _context;
        public EventLevelsController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        [HttpGet]
        public List<EventLevel> GetLevels(string query = null)
        {
            //Нужно добавить функцию лимит
            List<EventLevel> levels = _context.EventLevels.ToList();
            if (query != null) levels = levels.Where(e => e.Name.Contains(query)).ToList();
            return levels;
        }
    }
}
