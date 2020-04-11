using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using Microsoft.AspNet.Identity;
using System.Dynamic;
using System.ComponentModel;

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
        public ActionResult AllPeople(int? pageIndex, string sortBy)
        {
            if (!pageIndex.HasValue)
                pageIndex = 1;
            if (String.IsNullOrWhiteSpace(sortBy))
                sortBy = "FIO";
            string id = User.Identity.GetUserId();
            var personid = _context.Users.SingleOrDefault(u => u.Id == id).PersonId;
            //Если у пользователя роль "Руководитель отряда", тогда находим, какого отряда он сейчас является руководителем
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также дата окончания должности не равна null
            var headofsquad = _context.HeadsOfStudentSquads.SingleOrDefault(h => (h.PersonId == personid) && (h.DateofEnd==null));
            var squadId = headofsquad.SquadId;
            //Тут нужно получить запись (Person+(Member+Squad+Status)*необязательно)
            dynamic model = new ExpandoObject();
            var query = _context.People.Join(_context.Members.Include(m => m.Squad).Include(m => m.Status),
                p => p.Id,
                m => m.PersonId,
                (p, m) => new
                {
                    //Выход из организации
                    p.DateOfExit,
                    p.FIO,
                    p.DateofBirth,
                    p.PhoneNumber,
                    p.MembershipNumber,
                    SquadId = m.Squad.Id,
                    SquadName = (m == null ? String.Empty : m.Squad.Name),
                    StatusName = (m == null ? String.Empty : m.Status.Name),
                    //Выход из отряда
                    m.DateofTransition

                })
                //Только те, которые не перешли в другой отряд
                .Where(m => (m.DateofTransition == null)&&(m.SquadId==squadId))
                .OrderBy(m => m.SquadName)
                .ThenBy(p => p.FIO);
            List<ExpandoObject> joinData = new List<ExpandoObject>();

            foreach (var item in query)
            {
                IDictionary<string, object> itemExpando = new ExpandoObject();
                foreach (PropertyDescriptor property
                         in
                         TypeDescriptor.GetProperties(item.GetType()))
                {
                    itemExpando.Add(property.Name, property.GetValue(item));
                }
                joinData.Add(itemExpando as ExpandoObject);
                model.JoinData = joinData;
            }
            return View(model);
        }
        public ActionResult PersonForm()
        {
            var squads = _context.Squads.ToList();
            var viewModel = new NewPersonViewModel
            {
                Squads = squads
            };
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
                newModel.Person.FIO= Convert.ToString(newModel.Person.LastName + ' ' + newModel.Person.FirstName + ' ' + newModel.Person.PatronymicName);
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
                personInDb.PlaceofStudy = newModel.Person.PlaceofStudy;
                personInDb.FormofStudy = newModel.Person.FormofStudy;
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
                personInDb.FIO = Convert.ToString(newModel.Person.LastName +' '+ newModel.Person.FirstName+' '+ newModel.Person.PatronymicName);
            }
            _context.SaveChanges();
            
            return RedirectToAction("ShowAll","Members");
        }

    }
}