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
    public class WorksController : ApiController
    {
        private ApplicationDbContext _context;
        //Для обращения к методам Limit и GetHeadOfStudentSquads
        MembersController memberscontr = new MembersController();
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
            var headofsquad = _context.HeadsOfStudentSquads.Include(h => h.MainPosition).Include(h => h.Squad)
                .Include(h => h.UniversityHeadquarter).Include(h => h.RegionalHeadquarter)
                .SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Если активной записи о руководстве не найдено, перенаправляем на главную страницу
            return headofsquad;
        }
        [HttpPut]
        public List<Work> LimitWorks(List<Work> allworks, HeadsOfStudentSquads headofsquads)
        {
            List<Work> works = allworks;
            if (User.IsInRole("SquadManager"))
            {
                works = allworks.Where(w => w.Member.SquadId == headofsquads.SquadId).ToList();
            }
            else if (User.IsInRole("UniManager"))
            {
                string squadId = "";
                Squad squad = new Squad();
                foreach (var work in works)
                {
                    if (squadId != work.Member.SquadId.ToString())
                    {
                        squadId = work.Member.SquadId.ToString();
                        squad = _context.Squads.Include(s => s.UniversityHeadquarter).SingleOrDefault(s => s.Id == work.Member.SquadId);
                    }
                    if (squad.UniversityHeadquarterId == headofsquads.UniversityHeadquarterId)
                        works.Add(work);
                }
            }
            return works;
        }
        [HttpGet]
        public List<WorkViewModel> AllWorks(Guid? squadId = null, string season = null)
        {
            bool audit = false;
            List<WorkViewModel> listworks = new List<WorkViewModel>();
            var headofsquads = GetHeadOfStudentSquads();
            var allworks = _context.Works.Include(w => w.Member).Include(w => w.Employer).Include(w => w.WorkProject)
            .Where(w => w.Season == season).ToList();
            //Определяем по отрядам(Если отряд не определен, то текущий)
            List<Work> works = new List<Work>();
            if (squadId == null) works = LimitWorks(allworks, headofsquads);
            else works = allworks.Where(m => m.Member.SquadId == squadId).ToList();
            ////Ограничиваем по роли и принадлежности к отряду/штабу
            //List<Work> works = LimitWorks(allworks, headofsquads);
            //Формируем записи для представления
            string alternative = "";
            string affirmed = "";
            //Если текущий сезон, проверяем есть ли в записях внешний кноюч "Основная запись",
            //И, если есть, включаем аудит
            //При утверждения списка проставляем ОсновнойId
            if (works.Count > 0)
            {
                if (works[0].OriginalWorkId != null) audit = true;
            }
            //Если ведется Аудит
            if (audit)
            {
                //Очищаем works
                works = null;
                //В группы выделяем только те, которые не были отклонены
                var groups = works.Where(g => g.Approved != false).GroupBy(g => g.OriginalWorkId).ToList();
                foreach (var group in groups)
                {
                    //Находим максимум по дате создания в группе
                    var time = group.Max(n => n.CreateTime);
                    //Находим запись с такой датой создания
                    Work work = _context.Works.SingleOrDefault(w => (w.OriginalWorkId == group.Key) && (w.CreateTime == time));
                    //Добавляем только записи с последним изменением
                    works.Add(work);
                }
            }
            foreach (var work in works)
            {
                //Альтернатива
                if (work.Alternative) alternative = "Да";
                else alternative = "Нет";
                //Засчитана целина
                if (work.Affirmed == true) affirmed = "Засчитана";
                else if (work.Affirmed == false) affirmed = "Выговор";
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
                    DateofBeginString = work.DateofBegin.ToString("dd.MM.yyyy"),
                    DateofEndString = work.DateofEnd.ToString("dd.MM.yyyy"),
                    AlternativeString = alternative,
                    Affirmed = affirmed,
                };
                if (audit)
                {
                    //Проверяем есть ли неподтвержденные изменения
                    var changes = _context.Works.Where(c => (c.Approved == null) && (c.OriginalWorkId == work.Id));
                    if (changes.Count() > 0) workview.Changed = true;
                    else workview.Changed = false;
                }
                listworks.Add(workview);
            }
            return listworks;
        }
        [HttpPost]
        public IHttpActionResult CreateNewWork(WorkViewModel work)
        {
            foreach (var memberId in work.MembersIds)
            {
                Work newwork = new Work
                {
                    //DateTime2 нельзя ковертировать в DateTime, когда ты пытаещься нулевую дату вставить, когда nullable = false
                    CreateTime = DateTime.Now,
                    Id = Guid.NewGuid(),
                    MemberId = memberId,
                    EmployerId = work.EmployerId,
                    WorkProjectId = work?.WorkProjectId,
                    //Тут надо снова ошибку исправлять
                    DateofBegin = work.DateofBegin,
                    DateofEnd = work.DateofEnd,
                    Alternative = work.Alternative
                };
                _context.Works.Add(newwork);
            }
            _context.SaveChanges();
            return Ok();
        }
    
}
}
