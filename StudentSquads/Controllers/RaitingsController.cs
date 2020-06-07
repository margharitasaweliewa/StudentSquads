using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

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
                .Where(e => (e.Raiting.DateofCreation == null))
                .ToList();
            events = events.Where(e => (e.SquadId == headofsquads.SquadId)
                || (e.UniversityHeadquarterId == headofsquads.UniversityHeadquarterId) || (User.IsInRole("RegionalManager"))).ToList();
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
            var file = Request.Files.OfType<string>().FirstOrDefault();
            HttpPostedFileBase File = Request.Files[file] as HttpPostedFileBase;
            // получаем имя файла
            string fileName = Path.GetFileName(File.FileName);
            var fileId = Guid.NewGuid().ToString();
            var filePath = Path.Combine(fileId, fileName);
            // сохраняем файл в папку Files в проекте
         
            if (!Directory.Exists(Server.MapPath("~/Files/" + fileId )))
            {
                Directory.CreateDirectory(Server.MapPath("~/Files/" + fileId));
            }

            File.SaveAs(Server.MapPath("~/Files/" + filePath));

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
                _context.Raitings.Add(newraiting);
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
                    DocumentPath = filePath,
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
            foreach (var section in raitingsections)
            {
                //Находим все уровени, связанные с показателем
                var levels = _context.RaitingSectionLevels.Include(l => l.EventLevel)
                    .Where(l => l.RaitingSectionId == section.Id);
                //Создаем строку для всех уровней
                string alllevels = "";
                foreach (var level in levels)
                    if (level.EventLevel?.Name != null) { 
                    alllevels = alllevels + level.EventLevel.Name + ", ";
                alllevels = alllevels.Substring(0, alllevels.Length - 2);
                }
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
        public ActionResult EditSection(Guid id)
        {
            var section = _context.RaitingSections.Single(s => s.Id == id);
            var levels = _context.RaitingSectionLevels.Include(s => s.EventLevel)
                .Where(s => s.RaitingSectionId == id).ToList();
            var membershiptypes = _context.MembershipTypes.ToList();
            RaitingSectionViewModel viewModel = new RaitingSectionViewModel
            {
                Id = section.Id,
                Name = section.Name,
                Coef = section.Coef.ToString(),
                Levels = levels,
                MembershipTypeId = section.MembershipTypeId,
                MembershipTypes = membershiptypes
            };
            return View("RaitingSectionForm", viewModel);
        }
        public ActionResult AllRaitingEventInfos()
        {
            return View();
        }
        public ActionResult RaitingEventInfoForm()
        {
            //Находим все типы участия
            var membershiptypes = _context.MembershipTypes.ToList();
            RaitingEventInfoViewModel viewModel = new RaitingEventInfoViewModel
            {
                MembershipTypes = membershiptypes
            };
            return View(viewModel);
        }
        public ActionResult EditRaitingEventInfo(Guid id)
        {
            var viewModel = _context.RaitingEventInfos.Include(v => v.RaitingSection).Include(v => v.RaitingEvent)
                .Single(v => v.Id == id);
            var references = _context.RaitingEventInfoFiles.Where(v => v.RaitingEventInfoId == id).ToList();
            var membershiptypes = _context.MembershipTypes.ToList();
            //Составляем список ссылок
            Dictionary<string, string> referencedescriptions = new Dictionary<string, string>();
            foreach (var reference in references)
            {
                referencedescriptions.Add(reference.Path, reference.Description);
            }
            RaitingEventInfoViewModel raitinginfo = new RaitingEventInfoViewModel
            {
                Id = viewModel.Id,
                EventId = viewModel.RaitingEventId,
                MembershipCount = viewModel.MembershipCount.ToString(),
                MembershipTypes = membershiptypes,
                ReferenceDescriptions = referencedescriptions,
                MembershipTypeId = viewModel.RaitingSection.MembershipTypeId.ToString(),
                Event = viewModel.RaitingEvent.Name
            };
            return View("RaitingEventInfoForm", raitinginfo);
        }
        public ActionResult DeleteRaitingEventInfo(Guid id)
        {
            //Удаляем все связанные файлы
            var references = _context.RaitingEventInfoFiles.Where(r => r.RaitingEventInfoId == id).ToList();
            foreach (var reference in references)
            {
                _context.RaitingEventInfoFiles.Remove(reference);
            }
            //Удаляем сам Info
            var info = _context.RaitingEventInfos.Single(r => r.Id == id);
            _context.RaitingEventInfos.Remove(info);
            _context.SaveChanges();
            return RedirectToAction("AllRaitingEventInfos", "Raitings");
        }
        public ActionResult AllRaitingPlaces()
        {
            return View();
        }
       
        
    }
}