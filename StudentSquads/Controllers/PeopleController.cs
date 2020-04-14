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
using Microsoft.AspNet.Identity.EntityFramework;

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
            var personid = _context.People.SingleOrDefault(u => u.ApplicationUserId == id).Id;
            //Если у пользователя роль "Руководитель отряда", тогда находим, какого отряда он сейчас является руководителем
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также дата окончания должности не равна null
            var headofsquad = _context.HeadsOfStudentSquads.SingleOrDefault(h => (h.PersonId == personid) && (h.DateofEnd==null));
            if (headofsquad != null)
            {
                var squadId = headofsquad.SquadId;
                //Тут нужно получить запись (Person+(Member+Squad+Status)*необязательно)
                dynamic model = new ExpandoObject();
                DateTime dateofBirth = DateTime.Now;
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
                    .Where(m => (m.DateofTransition == null) && (m.SquadId == squadId))
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
            return RedirectToAction("Index","Home");
        }
        public ActionResult PersonForm()
        {//Определяем вид для отображения
            var squads = _context.Squads.ToList();
            var mainpositions = _context.MainPositions.ToList();
            var viewModel = new NewPersonViewModel
            {
                Squads = squads,
                MainPositions = mainpositions
            };
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Если User ещё не привязан к личности, возвращаем пустую форму
            if (person == null) return View(viewModel);
            //Иначе возвращаем заполненную
            else
            {

                var modelperson = _context.People.SingleOrDefault(p => p.Id == person.Id);
                viewModel.Person = modelperson;
                return View(viewModel);
            }

        }
        [HttpPost]
        public ActionResult Save(NewPersonViewModel newModel)
        {
            //Проверяем, есть ли личность у пользователя. Если нет, добавляем
            if (Convert.ToString(newModel.Person.Id) == "00000000-0000-0000-0000-000000000000")
            {
                if (!ModelState.IsValid)
                {
                    return View("PersonForm", newModel);
                }
                //Добавляем новую личность с идентификатором
                var personId = Guid.NewGuid();
                newModel.Person.Id = personId;
                newModel.Person.FIO = Convert.ToString(newModel.Person.LastName + ' ' + newModel.Person.FirstName + ' ' + newModel.Person.PatronymicName);
                //Получаем объект User
                string id = User.Identity.GetUserId();
                //Присваиваем Person
                newModel.Person.ApplicationUserId = id;
                //Если является членом организации, сразу проставляем дату вступления
                if (newModel.Person.MembershipNumber != null) newModel.Person.DateOfEnter = DateTime.Now;
                //Если является членом отряда, создаем запись в таблице "Member"
                if (newModel.Member.SquadId != null)
                {
                    newModel.Member.Id = Guid.NewGuid();
                    newModel.Member.PersonId = personId;
                    newModel.Member.DateofEnter = DateTime.Now;
                    newModel.Member.ApprovedByCommandStaff = true;
                    _context.Members.Add(newModel.Member);
                    //Если является ком. составом отряда,создаем запись в таблице "HeadsofStudentSquads"
                    if (newModel.HeadsOfStudentSquads.MainPositionId != null)
                    {
                        newModel.HeadsOfStudentSquads.Id = Guid.NewGuid();
                        newModel.HeadsOfStudentSquads.PersonId = personId;
                        newModel.HeadsOfStudentSquads.SquadId = newModel.Member.SquadId;
                        newModel.HeadsOfStudentSquads.DateofBegin = DateTime.Now;
                        _context.HeadsOfStudentSquads.Add(newModel.HeadsOfStudentSquads);
                        //Добавлеяем статус в зависимости от выбранного
                        //Очень опасный код, подумай, как исправить
                        var position = _context.MainPositions.Single(p => p.Id == newModel.HeadsOfStudentSquads.MainPositionId).Name;
                        int statusid = _context.Status.Single(s => s.Name == position).Id;
                        newModel.Member.StatusId = statusid;
                    }
                }
                _context.People.Add(newModel.Person);
                if (newModel.HeadsOfStudentSquads.MainPositionId != null)
                {
                    //И добавляем пользователю Роль "Руководитель отряда"
                    var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
                    userManager.AddToRole(id, "SquadManager");
                }
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
                personInDb.FIO = Convert.ToString(newModel.Person.LastName + ' ' + newModel.Person.FirstName + ' ' + newModel.Person.PatronymicName);
            }
            _context.SaveChanges();
            
            return RedirectToAction("AllPeople","People");
        }

    }
}