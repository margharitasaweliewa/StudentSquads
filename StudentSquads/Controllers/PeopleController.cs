using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentSquads.Models;
using StudentSquads.ViewModels;

namespace StudentSquads.Controllers
{
    public class PeopleController : Controller
    {
        private ApplicationDbContext _context;
        public PeopleController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: People
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewPerson()
        {
            var squads= _context.Squads.ToList();
            var viewModel = new NewPersonViewModel
            {
                Squads = squads
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Create(NewPersonViewModel newModel)
        {
            newModel.Person.Id = Guid.NewGuid();
            _context.People.Add(newModel.Person);
            _context.SaveChanges();
            
            return RedirectToAction("ShowAll","Members");
        }

    }
}