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
    public class OldPeopleController : ApiController
    {
        private ApplicationDbContext _context;
        public OldPeopleController()
        {
            _context = new ApplicationDbContext();
        }
        [HttpGet]
        public List<NewPersonViewModel> GetOldPeople()
        {
            //Находим руководящую позицию пользователя
            //var headofsquads = memberscontr.GetHeadOfStudentSquads();
            string id = User.Identity.GetUserId();
            var thisperson = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также должность активна
            var headofsquad = _context.HeadsOfStudentSquads.Include(h => h.MainPosition).Include(h => h.Squad)
                .Include(h => h.UniversityHeadquarter).Include(h => h.RegionalHeadquarter)
                .SingleOrDefault(h => (h.PersonId == thisperson.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            List<NewPersonViewModel> listofpeople = new List<NewPersonViewModel>();
            //Для рег. штаба находим людей, отчисленных из организации
            if (User.IsInRole("RegionalManager"))
            {
                //Находим всех исключенных членов
                var people = _context.People
                    .Where(p => (p.DateOfEnter != null) && (p.DateOfExit != null)).ToList();
                foreach (var person in people)
                {
                    //Находим последнего активного члена отряда для личности (который не перемещался в другой отряд)
                    var member = _context.Members.Include(m => m.Person).Include(m => m.Squad)
                        .SingleOrDefault(m => (m.PersonId == person.Id) && (m.DateOfEnter != null) && (m.ToSquadId == null));
                    string uni = "";
                    if (member != null)
                        uni = _context.Squads.Include(u => u.UniversityHeadquarter).Single(u => u.Id == member.SquadId).UniversityHeadquarter.ShortContent;
                    NewPersonViewModel newPerson = new NewPersonViewModel
                    {
                        Id = person.Id,
                        FIO = person.FIO,
                        DateofBirth = person.DateofBirth.ToString("dd.MM.yyyy"),
                        PhoneNumber = person.PhoneNumber,
                        MembershipNumber = person.MembershipNumber,
                        SquadName = member?.Squad.Name,
                        Uni = uni,
                        DateofEnterString = person.DateOfEnter?.ToString("dd.MM.yyyy"),
                        DateofExitString = person.DateOfEnter?.ToString("dd.MM.yyyy"),
                        ExitReason = person.ExitReason
                    };
                    listofpeople.Add(newPerson);

                }
            }
            //Иначе находим членов
            else
            {
                //Находим выбывших
                var members = _context.Members.Include(m => m.Squad).Include(m => m.Person)
                    .Where(m => (m.DateOfEnter != null) && (m.DateOfExit != null) &&
                    ((m.SquadId == headofsquad.SquadId) || (m.Squad.UniversityHeadquarterId == headofsquad.UniversityHeadquarterId))).ToList();
                foreach (var member in members)
                {
                    string uni = _context.Squads.Include(u => u.UniversityHeadquarter).Single(u => u.Id == member.SquadId).UniversityHeadquarter.ShortContent;
                    NewPersonViewModel newPerson = new NewPersonViewModel
                    {
                        Id = member.Person.Id,
                        FIO = member.Person.FIO,
                        DateofBirth = member.Person.DateofBirth.ToString("dd.MM.yyyy"),
                        PhoneNumber = member.Person.PhoneNumber,
                        MembershipNumber = member.Person.MembershipNumber,
                        SquadName = member.Squad.Name,
                        Uni = uni,
                        DateofEnterString = member.DateOfEnter?.ToString("dd.MM.yyyy"),
                        DateofExitString = member.DateOfEnter?.ToString("dd.MM.yyyy"),
                        ExitReason = member.ExitReason
                    };
                    listofpeople.Add(newPerson);
                }
            }
            return listofpeople;
        }
    }
}
