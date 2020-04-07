using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using Microsoft.AspNet.Identity;

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
        public ActionResult PersonForm()
        {
            var squads = _context.Squads.ToList();
            var viewModel = new NewPersonViewModel
            {
                Squads = squads
            };
            //Определяю Id текущего пользователя, чтобы найти id личности
            string id = User.Identity.GetUserId();
            var personid = _context.Users.SingleOrDefault(u => u.Id == id).PersonId;
            //Если User ещё не привязан к личности, возвращаем пустую форму
            if (personid is null)
                return View(viewModel);
            //Иначе возвращаем заполненную
            else
            {
                var person = _context.People.SingleOrDefault(p => p.Id == personid);
                viewModel.Person = person;
                return View(viewModel);
            };
        }
        [HttpPost]
        public ActionResult Create(NewPersonViewModel newModel)
        {
            newModel.Person.Id = Guid.NewGuid();
            _context.People.Add(newModel.Person);
            //Добавь ещё код изменения User, чтобы он ссылался на Person
            string id = User.Identity.GetUserId();
            var personid = _context.Users.SingleOrDefault(u => u.Id == id).PersonId;
            _context.SaveChanges();
            
            return RedirectToAction("ShowAll","Members");
        }
        public ActionResult Edit(Guid id)
        {
            var person = _context.People.SingleOrDefault(p => p.Id == id);
            if (person == null)
                return HttpNotFound();
            var viewModel = new NewPersonViewModel
            {
                Person = person,
                Squads = _context.Squads.ToList()

            };
            return View("NewPerson", viewModel);
        }

    }
}