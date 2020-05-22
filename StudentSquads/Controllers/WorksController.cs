using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNet.Identity;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
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
        [HttpPut]
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
            bool audit = false;
            //Проверяем, если одобренные записи в текущем сезона
            var works = _context.Works.Where(w => (w.Season == null)&&(w.Approved!=null)).ToList();
            //Если они есть, то должен производиться процесс аудита
            if (works.Count > 0) audit = true;
            WorkViewModel viewModel = new WorkViewModel {Audit = audit };
            return View(viewModel);
        }
        public ActionResult Edit(Guid id)
        {
            string view = "WorkEditForm";
            bool audit = false;
            Work work = _context.Works.Single(w => w.Id ==id);
            //Проверяем, если одобренные записи в текущем сезона
            var allworks = _context.Works.Where(w => (w.Season == null) && (w.Approved != null)).ToList();
            //Если они есть, то должен производиться процесс аудита
            if (allworks.Count != 0) audit = true;
            var works = _context.Works.Include(w => w.Member).Include(w => w.Employer).Include(w => w.WorkProject)
                .Include(w => w.WorkChangeType).Where(w => w.OriginalWorkId == work.OriginalWorkId).ToList();
            //Если нашли только 1 значение 
            if (works.Count == 1)work = works[0];
            //Есть записи аудита, надо найти самую старую и не отклоненную
            else
            {
                //Рассматриваем только неотклоненные
                var  approvedworks = works.Where(w => w.Approved != false).ToList();
                //Находим максимум по дате создания
                var time = approvedworks.Max(n => n.CreateTime);
                //Находим запись с такой датой создания
                List<Work> groupworks = _context.Works.Where(w => (w.OriginalWorkId == work.OriginalWorkId)).ToList();
                work = groupworks.Single(w => w.CreateTime == time);
                //Добавляем только записи с последним изменением
            }
            WorkViewModel viewModel = new WorkViewModel
            {
                Id = work.Id,
                PersonId = work.Member.PersonId,
                FIO = _context.Members.Include(p => p.Person)
                .SingleOrDefault(p => p.Id == work.MemberId).Person.FIO,
                Employer = work.Employer.Name,
                EmployerId = work.EmployerId,
                WorkProject = work.WorkProject?.Name,
                WorkProjectId = work.WorkProjectId,
                DateofBegin = work.DateofBegin,
                DateofEnd = work.DateofEnd,
                Alternative = work.Alternative,
                Audit = audit,
                MemberId = work.MemberId
            };
            if (audit)
            {
                //Создаем лист для версий
                List<WorkViewModel> versions = new List<WorkViewModel>();
                //Проходим по записям
                string approved = "Нет решения";
                foreach(var version in works) 
                {
                    if (version.Approved == true) approved = "Утверждено";
                    else if (version.Approved == false) approved = "Отклонено";
                    WorkViewModel workversion = new WorkViewModel
                    {
                        Id = version.Id,
                        PersonId = version.Member.PersonId,
                        FIO = _context.Members.Include(p => p.Person)
                    .SingleOrDefault(p => p.Id == version.MemberId).Person.FIO,
                        Employer = version.Employer.Name,
                        WorkProject = version.WorkProject?.Name,
                        DateofBeginString = version.DateofBegin.ToString("dd.MM.yyyy"),
                        DateofEndString = version.DateofEnd.ToString("dd.MM.yyyy"),
                        CreateTime = version.CreateTime.ToString("dd.MM.yyyy"),
                        Alternative = version.Alternative,
                        ChangeType = version.WorkChangeType?.Name,
                        ApprovedString = approved
                    };
                    versions.Add(workversion);
                }
                //Добавляем список версий
                viewModel.Versions = versions;
                //Настраиваем другое представление
                view = view + "Audit";
            }
            return View(view, viewModel);
        }
        // GET: Work
        public ActionResult AllWorks(string season=null)
        {
            return View();
        }
        //public ActionResult Delete(WorkViewModel work)
        //{
        //    var workInDb = _context.Works.Single(w => w.Id == work.Id);
        //    _context.Works.Remove(workInDb);
        //    _context.SaveChanges();
        //    return RedirectToAction("AllWorks","Works");
        //}
    }
}