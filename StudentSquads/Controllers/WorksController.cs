using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNet.Identity;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentSquads.Controllers
{
    public class WorksController : Controller
    {
        private ApplicationDbContext _context;
        public WorksController()
        {
            _context = new ApplicationDbContext();

        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public HeadsOfStudentSquads GetHeadOfStudentSquads()
        {
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также должность активна
            var headofsquad = _context.HeadsOfStudentSquads.Include(h => h.MainPosition)
                .SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Если активной записи о руководстве не найдено, перенаправляем на главную страницу
            return headofsquad;
        }
        MembersController memberscontr = new MembersController();
        public List<Work> LimitWorks(List<Work> allworks, HeadsOfStudentSquads headofsquads)
        {
            List<Work> works = new List<Work>();
            if (User.IsInRole("SquadManager"))
            { 
                works = allworks.Where(w => w.Member.SquadId == headofsquads.SquadId).ToList();
            }
            else if (User.IsInRole("UniManager"))
            {
                string squadId = "";
                Squad squad = new Squad();
                foreach(var work in works)
                {
                    if (squadId != work.Member.SquadId.ToString()) 
                    {
                        squadId = work.Member.SquadId.ToString();
                        squad = _context.Squads.Include(s => s.UniversityHeadquarter).SingleOrDefault(s => s.Id== work.Member.SquadId);
                    }
                    if(squad.UniversityHeadquarterId == headofsquads.UniversityHeadquarterId)
                    works.Add(work);
                }
            }
            return works;
        }
        public ActionResult WorkForm()
        {
            WorkViewModel viewModel = new WorkViewModel();
            return View(viewModel);
        }
        public ActionResult Edit(Guid id)
        {
            var work = _context.Works.Include(w => w.Member)
                .Include(w => w.Employer).Include(w => w.WorkProject).SingleOrDefault(w => w.Id ==id);
            WorkViewModel viewModel = new WorkViewModel
            {
                Id = work.Id,
                PersonId = work.Member.PersonId,
                FIO = _context.Members.Include(p => p.Person)
                .SingleOrDefault(p => p.Id == work.MemberId).Person.FIO,
                Employer = work.Employer.Name,
                WorkProject = work.WorkProject.Name,
                DateofBegin = work.DateofBegin,
                DateofEnd = work.DateofEnd,
                Alternative = work.Alternative,
            };
            return View();
        }
        // GET: Work
        public ActionResult AllWorks(string season=null)
        {
            List<WorkViewModel> listworks = new List<WorkViewModel>();
            var headofsquads = GetHeadOfStudentSquads();
            var allworks = _context.Works.Include(w => w.Member).Include(w => w.Employer).Include(w => w.WorkProject)
                .OrderBy(w => w.Member.SquadId).ToList();
            //Если нет указания сезона, тогда текущий (без указания)
            if (season == null) allworks = allworks.Where(w => w.Season == null).ToList();
            else allworks = allworks.Where(w => w.Season == season).ToList();
            //Ограничиваем по роли и принадлежности к отряду/штабу
            List<Work> works = LimitWorks(allworks, headofsquads);
            //Формируем записи для представления
            string alternative = "";
            string affirmed = "";
            foreach (var work in works)
            {
                //Альтернатива
                if (work.Alternative) alternative = "Да";
                else alternative = "Нет";
                //Засчитана целина
                if (work.Affirmed==true) affirmed = "Засчитана";
                else if (work.Affirmed==false)affirmed = "Выговор";
                else affirmed = "Нет решения";
                var member = _context.Members
                    .Include(m => m.Person).Include(m => m.Squad).SingleOrDefault(m => m.Id == work.MemberId);
                WorkViewModel workview = new WorkViewModel
                {
                    Id = work.Id,
                    PersonId = work.Member.PersonId,
                    FIO = member.Person.FIO,
                    Squad = member.Squad.Name,
                    Uni = _context.Squads.Include(u => u.UniversityHeadquarter)
                    .Single(u => u.Id == work.Member.SquadId).UniversityHeadquarter.University,
                    Employer = work.Employer.Name,
                    WorkProject = work.WorkProject?.Name,
                    DateofBegin = work.DateofBegin,
                    DateofEnd = work.DateofEnd,
                    AlternativeString = alternative,
                    Affirmed = affirmed
                };
                listworks.Add(workview);
            }
            return View(listworks);
        }
    }
}