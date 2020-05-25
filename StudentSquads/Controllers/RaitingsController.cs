using DocumentFormat.OpenXml.Office2010.PowerPoint;
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
    public class RaitingsController : Controller
    {
        private ApplicationDbContext _context;
        public RaitingsController()
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
        // GET: Raitings
        public ActionResult AllRaitingEvents()
        {
            List<RaitingEventViewModel> listofevents = new List<RaitingEventViewModel>();
            var headofsquads = GetHeadOfStudentSquads();
            //Отображаем только те, которые в текущем рейтинге, т е ещё не составленном DateofCreation = null
            //Отображаем утвержденные и созданные собственными руками
            var events = _context.RaitingEvents.Include(e => e.Raiting).Include(e => e.EventLevel)
                .Include(e => e.Squad).Include(e => e.UniversityHeadquarter).Include(e => e.RegionalHeadquarter)
                .Where(e => (e.Raiting.DateofCreation == null)
                &&((e.Approved==true)||(e.SquadId==headofsquads.SquadId)
                ||(e.UniversityHeadquarterId == headofsquads.UniversityHeadquarterId)||(User.IsInRole("RegionalManager"))))
                .ToList();
            foreach (var ev in events)
            {
                //Находим создателя
                string createdby = "";
                if (ev.SquadId != null) createdby = ev.Squad.Name;
                else if (ev.UniversityHeadquarterId != null) createdby = ev.UniversityHeadquarter.University;
                else if (ev.RegionalHeadquarterId != null) createdby = ev.RegionalHeadquarter.Region;
                //Находим утверждение
                string approved = "Нет решения";
                if (ev.Approved == true) approved = "Утверждено";
                else if (ev.Approved == false) approved = "Отклонено";
                RaitingEventViewModel raitingevent = new RaitingEventViewModel
                {
                    Id = ev.Id,
                    Name = ev.Name,
                    EventLevel = ev.EventLevel.Name,
                    CreatedBy = createdby,
                    Approved = approved,
                    DateofBeginString = ev.DateofBegin.ToString("dd.MM.yyyy"),
                    DateofEndString = ev.DateofEnd.ToString("dd.MM.yyyy"),
                    DocumentPath = ev.DocumentPath
                };
                listofevents.Add(raitingevent);
            }
            return View(listofevents);
        }
        public ActionResult SaveEvent(RaitingEventViewModel model)
        {
            Guid raitingId = Guid.Empty;
            //Проверяем, создан ли текйщий рейтинг
            var raiting = _context.Raitings.SingleOrDefault(r => r.DateofCreation == null);
            //Если текущй рейтинг не создан, создаем его
            if (raiting == null)
            {
                Raiting newraiting = new Raiting
                {
                    Id = Guid.NewGuid(),
                    DateofBegin = DateTime.Now
                };
                raitingId = newraiting.Id;
            }
            //Иначе берем текущий
            else raitingId = raiting.Id;
            //При создании
            if (model.Id == Guid.Empty)
            {
                bool? approved = null;
                //Узнаем, какой пользователь
                var headofsquads = GetHeadOfStudentSquads();
                //Если рег. отделение создает мероприятие, оно сразу утверждается
                if (User.IsInRole("RegionalManager")) approved = true;
                RaitingEvent newevent = new RaitingEvent
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    DateofBegin = model.DateofBegin,
                    DateofEnd = model.DateofEnd,
                    SquadId = headofsquads.SquadId,
                    UniversityHeadquarterId = headofsquads.UniversityHeadquarterId,
                    RegionalHeadquarterId = headofsquads.RegionalHeadquarterId,
                    Approved = approved,
                    EventLevelId = model.EventLevelId,
                    RaitingId = raitingId
                    //Надо ещё с документами поработать
                };
                _context.RaitingEvents.Add(newevent);
            }
            else
            {
                var raitingeventInDb = _context.RaitingEvents.Single(r => r.Id == model.Id);
                if (model.DateofBegin.ToString("dd.MM.yyyy") != "01.01.0001")
                    raitingeventInDb.DateofBegin = model.DateofBegin;
                if (model.DateofEnd.ToString("dd.MM.yyyy") != "01.01.0001")
                    raitingeventInDb.DateofEnd = model.DateofEnd;
                raitingeventInDb.EventLevelId = model.EventLevelId;
                raitingeventInDb.Name = model.Name;
            }
            _context.SaveChanges();
            return RedirectToAction("AllRaitingEvents", "Raitings");
        }
        public ActionResult RaitingEventForm()
        {
            var eventlevels = _context.EventLevels.ToList();
            var viewModel = new RaitingEventViewModel
            {
                EventLevels = eventlevels
            };
            return View(viewModel);
        }
        public ActionResult EditEvent(Guid id)
        {
            var eventslevels = _context.EventLevels.ToList();
            RaitingEvent raitingevent = _context.RaitingEvents.SingleOrDefault(e => e.Id == id);
            RaitingEventViewModel viewModel = new RaitingEventViewModel
            {
                Id = raitingevent.Id,
                DateofBegin = raitingevent.DateofBegin,
                DateofEnd = raitingevent.DateofEnd,
                EventLevelId = raitingevent.EventLevelId,
                Name = raitingevent.Name,
                DateofBeginString = raitingevent.DateofBegin.ToString("dd.MM.yyyy"),
                DateofEndString = raitingevent.DateofEnd.ToString("dd.MM.yyyy"),
                EventLevels = eventslevels
            };
            return View("RaitingEventForm", viewModel);
        }
        public ActionResult DeleteEvent(Guid id)
        {
            var ev = _context.RaitingEvents.Single(r => r.Id == id);
            _context.RaitingEvents.Remove(ev);
            _context.SaveChanges();
            return RedirectToAction("AllRaitingEvents", "Raitings");
        }
        public ActionResult ApproveEvent(Guid id)
        {
            var ev = _context.RaitingEvents.Single(r => r.Id == id);
            ev.Approved = true;
            _context.SaveChanges();
            return RedirectToAction("AllRaitingEvents", "Raitings");
        }
        public ActionResult RejectEvent(Guid id)
        {
            var ev = _context.RaitingEvents.Single(r => r.Id == id);
            ev.Approved = false;
            _context.SaveChanges();
            return RedirectToAction("AllRaitingEvents", "Raitings");
        }
        public ActionResult AllRaitingSections()
        {
            List<RaitingSectionViewModel> listsections = new List<RaitingSectionViewModel>();
            //Выбираем все показатели
            var raitingsections = _context.RaitingSections.Include(r => r.MembershipType)
                .ToList();
            foreach(var section in raitingsections)
            {
                //Находим все уровени, связанные с показателем
                var levels = _context.RaitingSectionLevels.Include(l => l.EventLevel)
                    .Where(l => l.RaitingSectionId == section.Id);
                //Создаем строку для всех уровней
                string alllevels = "";
                foreach(var level in levels)
                    alllevels = alllevels + level.EventLevel.Name + ";";
                //Находим статус
                string status = "Активно";
                if (section.Removed) status = "Удалено";
                RaitingSectionViewModel newsection = new RaitingSectionViewModel
                {
                    Id = section.Id,
                    Name = section.Name,
                    MembershipType = section.MembershipType.Name,
                    Level = alllevels,
                    Status = status,
                    Coef = section.Coef.ToString()
                };
                listsections.Add(newsection);
            }
            listsections.OrderBy(l => l.Name);
            return View(listsections);
        }
        public ActionResult RaitingSectionForm()
        {
            //Находим все типы участия
            var membershiptypes = _context.MembershipTypes.ToList();
            RaitingSectionViewModel viewModel = new RaitingSectionViewModel
            {
                MembershipTypes = membershiptypes
            };
            return View(viewModel);
        }
    }
}