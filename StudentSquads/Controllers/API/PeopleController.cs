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
    public class PeopleController : ApiController
    {
        private ApplicationDbContext _context;
        public PeopleController()
        {
            _context = new ApplicationDbContext();
        }
        //Для обращения к методам Limit и GetHeadOfStudentSquads
        MembersController memberscontr = new MembersController();
        //GET /api/people
        public List<NewPersonViewModel> GetPeople()
        {
            string uni = "";
            Guid? uniId = new Guid();
            //Для формирования списка
            List<NewPersonViewModel> listofmembers = new List<NewPersonViewModel>();
            //Находим руководящую позицию пользователя
            var headofsquads = memberscontr.GetHeadOfStudentSquads();
            //Для найденных members
            List<Member> members = _context.Members.Include(m => m.Person).Include(m => m.Squad).Include(m => m.Status)
                    .Where(m => (m.DateOfEnter != null) && (m.DateOfExit == null) &&(m.Person.DateOfExit==null))
                    .OrderBy(m => m.Squad.UniversityHeadquarterId).ToList();
            //Выделяем список только под Id личностей
            var personmembers = members.Select(p => p.PersonId).ToList();
            members = memberscontr.LimitMembers(members, headofsquads);
            if (User.IsInRole("UniManager"))
            {
                //Найдем всех руководителей штаба
                var allheads = _context.HeadsOfStudentSquads.Include(h => h.Person).Include(h => h.UniversityHeadquarter)
                    .Where(h => h.UniversityHeadquarterId==headofsquads.UniversityHeadquarterId).ToList();
                //Формируем список, если PersonId совпадают, оставляем информацию о принадлежности отряду
                foreach (var member in allheads)
                {
                    //Если человек в обоих списках, то не добавляем
                    if (!personmembers.Exists(x => x.Equals(member.PersonId)))
                    {
                        NewPersonViewModel newmember = new NewPersonViewModel
                        {
                            Id = member.Person.Id,
                            FIO = member.Person.FIO,
                            DateofBirth = member.Person.DateofBirth.ToString("dd.MM.yyyy"),
                            PhoneNumber = member.Person.PhoneNumber,
                            MembershipNumber = member.Person.MembershipNumber,
                            Uni = headofsquads.UniversityHeadquarter.University
                        };
                        listofmembers.Add(newmember);
                    }
                }
            }
            else if (User.IsInRole("RegionalManager"))
            {
                //Берем руководителей, исключаем руководителей отрядов, так как они 100% попали в список членов организации
                var allheads = _context.HeadsOfStudentSquads.Include(h => h.Person).Include(h => h.UniversityHeadquarter)
                    .Where(h => h.SquadId==null).ToList();
                foreach (var member in allheads)
                {
                    //Если человек в обоих списках, то не добавляем
                    if (!personmembers.Exists(x => x.Equals(member.PersonId)))
                    {
                        //Если руководитель штаба, то находим название штаба
                        if (member.UniversityHeadquarterId != null) 
                        {
                            uni = _context.UniversityHeadquarters.SingleOrDefault(u => u.Id == member.UniversityHeadquarterId).University;
                        };
                        NewPersonViewModel newmember = new NewPersonViewModel
                        {
                            Id = member.Person.Id,
                            FIO = member.Person.FIO,
                            DateofBirth = member.Person.DateofBirth.ToString("dd.MM.yyyy"),
                            PhoneNumber = member.Person.PhoneNumber,
                            MembershipNumber = member.Person.MembershipNumber,
                            Uni = uni,
                        };
                        listofmembers.Add(newmember);
                    }
                }
            }
            foreach (var member in members)
            {
                if (uniId != member.Squad.UniversityHeadquarterId)
                {
                    uniId = member.Squad.UniversityHeadquarterId;
                    uni = _context.UniversityHeadquarters.SingleOrDefault(u => u.Id == member.Squad.UniversityHeadquarterId).University; 
                }
                NewPersonViewModel newmember = new NewPersonViewModel
                    {
                        Id = member.Person.Id,
                        FIO = member.Person.FIO,
                        DateofBirth = member.Person.DateofBirth.ToString("dd.MM.yyyy"),
                        PhoneNumber = member.Person.PhoneNumber,
                        MembershipNumber = member.Person.MembershipNumber,
                        SquadId = member.Squad.Id,
                        SquadName = member.Squad.Name,
                        StatusName = (member.StatusId == null ? String.Empty : member.Status.Name),
                        Uni = uni
                    };
                    listofmembers.Add(newmember);
            }
            //dynamic model = new ExpandoObject();
            //var query = members.Join(_context.UniversityHeadquarters,
            //    m => m.Squad.UniversityHeadquarterId,
            //    u => u.Id,
            //    (m, u) => new
            //    {
            //        m.Person.Id,
            //        m.Person.FIO,
            //        m.Person.DateofBirth,
            //        m.Person.PhoneNumber,
            //        m.Person.MembershipNumber,
            //        SquadId = m.Squad.Id,
            //        SquadName = m.Squad.Name,
            //        StatusName = (m.Status == null ? String.Empty : m.Status.Name),
            //        //Выход из отряда
            //    })
            //    //Только те, которые не вышли из отряда
            //    .OrderBy(m => m.SquadName)
            //    .ThenBy(p => p.FIO);
            //List<ExpandoObject> joinData = new List<ExpandoObject>();
            ////Сначала обрабатываем ExpandoObject
            //foreach (var item in query)
            //{
            //    IDictionary<string, object> itemExpando = new ExpandoObject();
            //    foreach (PropertyDescriptor property
            //             in
            //             TypeDescriptor.GetProperties(item.GetType()))
            //    {
            //        itemExpando.Add(property.Name, property.GetValue(item));
            //    }
            //    joinData.Add(itemExpando as ExpandoObject);
            //    model.JoinData = joinData;
            //}
            ////Затем переводим ExpandoObject в обычный класс
            //foreach (var member in model.JoinData)
            //{
            //    NewPersonViewModel newmember = new NewPersonViewModel
            //    {
            //        Id = member.Id,
            //        SquadId = member.SquadId,
            //        FIO = member.FIO,
            //        DateofBirth = member.DateofBirth.ToString("dd.MM.yyyy"),
            //        PhoneNumber = member.PhoneNumber,
            //        MembershipNumber = member.MembershipNumber,
            //        SquadName = member.SquadName,
            //        StatusName = member.StatusName
            //    };
            //    listofmembers.Add(newmember);
            //}
            return listofmembers;
        }
        // GET /api/people/id
        public Person GetPerson(Guid id)
        {
            var person = _context.People.SingleOrDefault(p => p.Id == id);
            if (person == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return person;
        }

        // POST /api/people
        [HttpPost]
        public NewPersonViewModel CreatePerson(NewPersonViewModel newModel)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);
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
                newModel.Member.DateOfEnter = DateTime.Now;
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
                    //Добавлеяем статус "Член ком. состава"
                    newModel.Member.StatusId = 8;
                }
            }
            _context.People.Add(newModel.Person);
            if (newModel.HeadsOfStudentSquads.MainPositionId != null)
            {
                //И добавляем пользователю Роль "Руководитель отряда"
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
                userManager.AddToRole(id, "SquadManager");
            }
            _context.SaveChanges();
            return newModel;
        }
        //PUT /api/people/id
        [HttpPut]
        public void UpdatePerson(NewPersonViewModel newModel)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            var personInDb = _context.People.Single(p => p.Id == newModel.Person.Id);
            if (personInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

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
            personInDb.PassportSerie = newModel.Person.PassportSerie;
            personInDb.PassportNumber = newModel.Person.PassportNumber;
            personInDb.RegistrationPlace = newModel.Person.RegistrationPlace;
            personInDb.Sex = newModel.Person.Sex;
            personInDb.Snils = newModel.Person.Snils;
            personInDb.FIO = Convert.ToString(newModel.Person.LastName + ' ' + newModel.Person.FirstName + ' ' + newModel.Person.PatronymicName);
            _context.SaveChanges();
        }
        // DELETE /api/people/1
        [HttpDelete]
        public IHttpActionResult ExcludeMember(Guid id, string reason)
        {
            var personInDb = _context.People.SingleOrDefault(p => p.Id == id);
            if (personInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            var memberInDb = _context.Members.SingleOrDefault(m => (m.PersonId == id) && (m.DateOfExit == null));
            //При исключении подставляем дату исключения
            personInDb.DateOfExit = DateTime.Now;
            personInDb.ExitReason = reason;
            memberInDb.DateOfExit = DateTime.Now;
            memberInDb.ExitReason = reason;
            _context.SaveChanges();

            return Ok();
        }

    }
}
