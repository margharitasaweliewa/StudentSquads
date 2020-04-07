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
        public ActionResult Save(NewPersonViewModel newModel)
        {
            if (Convert.ToString(newModel.Person.Id)== "00000000-0000-0000-0000-000000000000")
            {
                //Добавляем новую личность с идентификатором
                var personId = Guid.NewGuid();
                newModel.Person.Id = personId;
                _context.People.Add(newModel.Person);
                //Получаем объект User, присваиваем ему PersonId
                string id = User.Identity.GetUserId();
                var user = _context.Users.SingleOrDefault(u => u.Id == id);
                user.PersonId = personId;
            }
            else
            {
                var personInDb = _context.People.Single(p => p.Id == newModel.Person.Id);
                //Изменяю поля персональных данных
                personInDb.LastName = newModel.Person.LastName;
                personInDb.FirstName = newModel.Person.FirstName;
                personInDb.PatronymicName = newModel.Person.PatronymicName;
                personInDb.PhoneNumber = newModel.Person.PhoneNumber;
                personInDb.CityofBirth = newModel.Person.CityofBirth;
                personInDb.DateofBirth = newModel.Person.DateofBirth;
                personInDb.DateofIssue = newModel.Person.DateofIssue;
                personInDb.DepartmentCode = newModel.Person.DepartmentCode;
                personInDb.Email = newModel.Person.Email;
                personInDb.INN = newModel.Person.INN;
                personInDb.PasportSerie = newModel.Person.PasportSerie;
                personInDb.PassportNumber = newModel.Person.PassportNumber;
                personInDb.RegistrationPlace = newModel.Person.RegistrationPlace;
                personInDb.Sex = newModel.Person.Sex;
                personInDb.Snils = newModel.Person.Snils;
            }
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