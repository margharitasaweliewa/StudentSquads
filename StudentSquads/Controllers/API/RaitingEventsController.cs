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
    public class RaitingEventsController : ApiController
    {
        private ApplicationDbContext _context;
        public RaitingEventsController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        [HttpGet]
        public List<RaitingEventViewModel> AllRaitingEvents(string query = null)
        {
            List<RaitingEventViewModel> listofevents = new List<RaitingEventViewModel>();
            //Отображаем только те, которые в текущем рейтинге, т е ещё не составленном DateofCreation = null
            var events = _context.RaitingEvents.Include(e => e.Raiting).Include(e => e.EventLevel)
                .Include(e => e.Squad).Include(e => e.UniversityHeadquarter).Include(e => e.RegionalHeadquarter)
                .Where(e => e.Raiting.DateofCreation==null).ToList();
            if (query != null) events = events.Where(e => e.Name.Contains(query)).ToList();
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
            return listofevents;

        }
    }
}
