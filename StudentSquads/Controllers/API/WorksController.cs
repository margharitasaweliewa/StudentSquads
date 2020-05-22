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
using System.Web.Services.Protocols;
using System.Data;

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
        //[HttpGet]
        //public List<Work> LimitWorks(List<Work> allworks, HeadsOfStudentSquads headofsquads)
        //{
        //    List<Work> works = allworks;
        //    if (User.IsInRole("SquadManager"))
        //    {
        //        works = allworks.Where(w => w.Member.SquadId == headofsquads.SquadId).ToList();
        //    }
        //    else if (User.IsInRole("UniManager"))
        //    {
        //        string squadId = "";
        //        Squad squad = new Squad();
        //        foreach (var work in works)
        //        {
        //            if (squadId != work.Member.SquadId.ToString())
        //            {
        //                squadId = work.Member.SquadId.ToString();
        //                squad = _context.Squads.Include(s => s.UniversityHeadquarter).SingleOrDefault(s => s.Id == work.Member.SquadId);
        //            }
        //            if (squad.UniversityHeadquarterId == headofsquads.UniversityHeadquarterId)
        //                works.Add(work);
        //        }
        //    }
        //    return works;
        //}
        [HttpGet]
        public List<WorkViewModel> AllWorks(Guid? squadId = null, int? season = null)
        {
            bool audit = false;
            List<WorkViewModel> listworks = new List<WorkViewModel>();
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также должность активна
            var headofsquads= _context.HeadsOfStudentSquads.Include(h => h.MainPosition).Include(h => h.Squad)
                .Include(h => h.UniversityHeadquarter).Include(h => h.RegionalHeadquarter)
                .SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Смотрим только текущий сезон
            var allworks = _context.Works.Include(w => w.Member).Include(w => w.Employer).Include(w => w.WorkProject).Include(w => w.WorkChangeType)
            .Where(w => w.Season == season).ToList();
            //Определяем по отрядам(Если отряд не определен, то текущий)
            List<Work> works = new List<Work>();
            if (squadId == null)
            //Вставляю здесь, потому что не могу разобраться, как правильно функции писать
            {
                if (User.IsInRole("SquadManager"))
                {
                    works = allworks.Where(w => w.Member.SquadId == headofsquads.SquadId).ToList();
                }
                else if (User.IsInRole("UniManager"))
                {
                    List<Work> newworks = new List<Work>();
                    string squadIdstr = "";
                    Squad squad = new Squad();
                    foreach (var work in works)
                    {
                        //Проверяем, рассматриваем ли мы отряд текущего члена организации
                        if (squadIdstr != work.Member.SquadId.ToString())
                        {
                            //Если нет, то текущий отряд = отряд текущего членв
                            squadIdstr = work.Member.SquadId.ToString();
                            squad = _context.Squads.Include(s => s.UniversityHeadquarter).SingleOrDefault(s => s.Id == work.Member.SquadId);
                        }
                        //Если отряд относится к штабу, тогда добавляем информацт=ию о работе в список
                        if (squad.UniversityHeadquarterId == headofsquads.UniversityHeadquarterId)
                            newworks.Add(work);
                    }
                }
                else if (User.IsInRole("RegionalManager"))
                {
                    works = allworks;
                }
            }
            else works = allworks.Where(m => m.Member.SquadId == squadId).ToList();
            //Формируем записи для представления
            string alternative = "";
            string affirmed = "";
            //Проверяем, утвержден ли список
            var worksapproved = works.Where(w => w.Approved != null).ToList();
            //Включаем аудит, если утвержден
            if (worksapproved.Count()>0) audit = true;
            //Если ведется Аудит
            if (audit)
            {
                //В группы выделяем только те, которые не были отклонены и которые не были удалены
                var groups = works.Where(g => (g.Approved != false)&&(g.Season == null)&&(g.Removed==null))
                    .GroupBy(g => g.OriginalWorkId).ToList();
                //Очищаем works
                works = new List<Work>();
                //Добавляем к работам на целине записи с последними неотклоненными изменениями
                foreach (var group in groups)
                {
                    //Находим максимум по дате создания в группе
                    var time = group.Max(n => n.CreateTime);
                    List<Work> groupworks = _context.Works.Where(w => (w.OriginalWorkId == group.Key)).ToList();
                    Work work = groupworks.Single(w => w.CreateTime==time);
                    //Добавляем только записи с последним изменением
                    works.Add(work);
                }

            }
            //Идем по образованному списку
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
                    Season = work.Season.ToString(),
                    ChangeType = work.WorkChangeType?.Name,
                    
                };
                if (audit)
                {
                    //Проверяем есть ли неподтвержденные изменения
                    //Для новых данных отдельное условие - Тип изменения - "добавление"
                    var changes = _context.Works.Where(c => (c.Approved == null) && ((c.OriginalWorkId == work.Id)||((c.Id ==work.Id)&&(c.WorkChangeTypeId==1))));
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
            //При создании новых элементов
            if (work.Id == Guid.Empty)
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
                        Alternative = work.Alternative,
                        AlternativeReason = work.AlternativeReason
                    };
                    //Это первоначальная запись
                    newwork.OriginalWorkId = newwork.Id;
                    //Если запись создается после утверждения, то мы присваиваем тип "Добавление"
                    if (work.Audit) newwork.WorkChangeTypeId = 1;
                    _context.Works.Add(newwork);
                }
            }
            //Зашли в созданную ранее запись
            else
            {
                    //Если список уже утвержден, создаем новую запись
                    if (work.Audit)
                    {
                    var workInDb = _context.Works.Include(w => w.Member).Include(w => w.WorkProject).Include(w => w.Employer)
    .SingleOrDefault(w => w.Id == work.Id);
                    //Если какое-либо поле не было заполнено, заполняем его старым значением
                    if (work.DateofBegin.ToString("dd.MM.yyyy") == "01.01.0001")
                        work.DateofBegin = workInDb.DateofBegin;
                    if (work.DateofEnd.ToString("dd.MM.yyyy") == "01.01.0001")
                        work.DateofEnd = workInDb.DateofEnd;
                    Work newwork = new Work
                        {
                            //DateTime2 нельзя ковертировать в DateTime, когда ты пытаещься нулевую дату вставить, когда nullable = false
                            CreateTime = DateTime.Now,
                            Id = Guid.NewGuid(),
                            MemberId = work.MemberId,
                            EmployerId = work.EmployerId,
                            WorkProjectId = work?.WorkProjectId,
                            DateofBegin = work.DateofBegin,
                            DateofEnd = work.DateofEnd,
                            Alternative = work.Alternative,
                            //При создании записи после утверждения списка - тип "Изменение"
                            WorkChangeTypeId = 1004,
                            OriginalWorkId = work.OriginalWorkId
                        };
                        _context.Works.Add(newwork);
                 }
                //Если ещё не утверждено, тогда редактируем просто старую запись
                else
                {
                    //Находим текущую запись в БД
                    var workInDb = _context.Works.Include(w => w.Member).Include(w => w.WorkProject).Include(w => w.Employer)
                        .SingleOrDefault(w => w.Id == work.Id);
                    //Если введена новая дата
                    if(work.DateofBegin.ToString("dd.MM.yyyy")!="01.01.0001")
                    workInDb.DateofBegin = work.DateofBegin;
                    if (work.DateofBegin.ToString("dd.MM.yyyy") != "01.01.0001")
                        workInDb.DateofEnd = work.DateofEnd;
                    //Если передали новое значение, то мы его изменяем
                    if(work.EmployerId!=Guid.Empty)
                    workInDb.EmployerId = work.EmployerId;
                    if(work.WorkProjectId!=Guid.Empty)
                    workInDb.WorkProjectId = work.WorkProjectId;
                    workInDb.Alternative = work.Alternative;
                    workInDb.AlternativeReason = work.AlternativeReason;
                }
            }
            _context.SaveChanges();
            return Ok();
        }
        [HttpPut]
        public IHttpActionResult ApproveList()
        {
            //Все записи в списке текущего сезона
            var works = _context.Works.Where(w => w.Season == null).ToList();
            foreach (var workInDb in works)
            {
                workInDb.Approved = true;
            }
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete]
        public IHttpActionResult DeleteWork(Guid id, string reason=null)
        {
                var work = _context.Works.Single(w => w.Id ==id);
                if(reason!=null)
            {
                Work newwork = new Work
                {
                    //DateTime2 нельзя ковертировать в DateTime, когда ты пытаещься нулевую дату вставить, когда nullable = false
                    CreateTime = DateTime.Now,
                    Id = Guid.NewGuid(),
                    MemberId = work.MemberId,
                    EmployerId = work.EmployerId,
                    WorkProjectId = work.WorkProjectId,
                    DateofBegin = work.DateofBegin,
                    DateofEnd = work.DateofEnd,
                    Alternative = work.Alternative,
                    AlternativeReason = work.AlternativeReason,
                    WorkChangeTypeId = 2,
                    OriginalWorkId = work.OriginalWorkId,
                    ExitReason = reason,
                    Approved = false
                    
                };
                _context.Works.Add(newwork);

            }
            else
            {
                _context.Works.Remove(work);
                _context.SaveChanges();
            }
            _context.SaveChanges();

            return Ok();
        }

    }
}
