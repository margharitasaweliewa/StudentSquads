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
using DocumentFormat.OpenXml.EMMA;

namespace StudentSquads.Controllers.API
{
    public class RaitingPlacesController : ApiController
    {
        private ApplicationDbContext _context;
        public RaitingPlacesController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        [HttpGet]
        public List<RaitingPlaceViewModel> AllRaitingPlaces()
        {
            List<RaitingPlaceViewModel> listplaces = new List<RaitingPlaceViewModel>();
            var places = _context.RaitingPlaces.Include(p => p.Raiting).Include(p => p.Squad).ToList();
            //Группируем по рейтингу
            var groupplaces = places.GroupBy(p => p.Raiting.Id);
            //Идем по каждому рейтингу
            foreach (var group in groupplaces)
            {
                //Находим записи данного рейтинга
                var raitingplaces = places.Where(p => p.RaitingId == group.Key).ToList();
                //Группируем по отряду
                var groupsquad = raitingplaces.GroupBy(r => r.SquadId).ToList();
                foreach(var squad in groupsquad)
                {
                    var oneplace = raitingplaces
                        .Where(r => r.SquadId == squad.Key).ToList();
                    Guid? squadid = oneplace[0].SquadId;
                    string uni = _context.Squads
                                 .Include(s => s.UniversityHeadquarter)
                                  .Single(s => s.Id == squadid)
                                  .UniversityHeadquarter.University;
                    RaitingPlaceViewModel newplace = new RaitingPlaceViewModel
                    {
                        SquadId = oneplace[0].SquadId,
                        Squad = oneplace[0].Squad.Name,
                        MainPlace = oneplace[0].MainPlace,
                        Uni = uni,
                        Raiting = oneplace[0].Raiting.DateofCreation?.ToString("dd.MM.yyyy"),
                        RaitingDate = oneplace[0].Raiting.DateofCreation,
                        
                    };
                    newplace.MainPlace = oneplace[0].MainPlace;
                    listplaces.Add(newplace);
                }
            }
            //Сортируем по дате, потом по отряду
            listplaces.OrderByDescending(l => l.RaitingDate).ThenBy(l => l.Squad);
            return listplaces;
        }
        [HttpPost]
        public IHttpActionResult MakeRaitingList()
        {
            //Находим текущий рейтинг
            var raiting = _context.Raitings.SingleOrDefault(r => r.DateofCreation == null);
            if (raiting == null) return BadRequest();
            //Идем по каждому неудаленному показателю
            var sections = _context.RaitingSections.Where(r => r.Removed != true).ToList();
            foreach (var section in sections)
            {
                //Группируем материалы по отрядом с условием, что оно относится к этому показателю
                //Также материалы должны относиться к текущему рейтингу
                //Также материалы должны быть одобрены
                //Сначала собираем все материалы
                var allinfos = _context.RaitingEventInfos.Include(r => r.RaitingEvent)
                    .Where(r => (r.RaitingSectionId == section.Id) && (r.RaitingEvent.RaitingId == raiting.Id) && (r.Approved == true))
                    .ToList();
                var groups = allinfos.GroupBy(r => r.SquadId).ToList();
                //Создаем словарь по показателю для записи баллов
                //Guid отряда, количество баллов
                Dictionary<Guid?, double> dict = new Dictionary<Guid?, double>();
                //Идем по отряду по каждому выложенному материалу
                foreach (var group in groups)
                {
                    //Находим активное кол-во членов отряда
                    int generallcount = _context.Members.Where(m => (m.SquadId == group.Key) && (m.DateOfEnter != null) && (m.DateOfExit == null)).ToList().Count();
                    //Выбираем только материалы данного отряда
                    var groupinfos = allinfos.Where(i => i.SquadId == group.Key);
                    //Определяем переменную для определения балла
                    double points = 0;
                    //Идем по каждому материалы
                    foreach (var info in groupinfos)
                    {
                        //Количество участников
                        double memberscount = info.MembershipCount;
                        double point = (memberscount / generallcount);
                        //Складываем баллы
                        points = points + point;
                    }
                    //Добавляем в Dictionary значение
                    dict.Add(group.Key, points);
                }
                //Создаем переменную для обозначения места
                int place = 1;
                //Сортируем Dictionry по значению
                var groupdict = dict.OrderByDescending(pair => pair.Value).ToList();
                //Находим количество баллов у первого места
                double oldpoints = groupdict[0].Value;
                foreach (var pair in groupdict)
                {
                    double thispoints = pair.Value;
                    //Увеличиваем место, если баллы не совпадают
                    if (thispoints != oldpoints) place++;
                    //Делаем старыми баллами текущие баллы
                    oldpoints = thispoints;
                    //Создаем в базе данных запись с метом в показателе
                    RaitingPlace newplace = new RaitingPlace
                    {
                        Id = Guid.NewGuid(),
                        Place = place,
                        Points = pair.Value,
                        RaitingSectionId = section.Id,
                        RaitingId = raiting.Id,
                        SquadId = pair.Key
                    };
                    //Добавляем в БД
                    _context.RaitingPlaces.Add(newplace);
                }
            }
            //Сохраняем добавленные места
            _context.SaveChanges();
            //Вычисляем основные места
            //Создаем словарь для отряд - общий балл
            Dictionary<Guid?, double> maindict = new Dictionary<Guid?, double>();
            //Находим записи в текущем рейтинге
            var raitingplaces = _context.RaitingPlaces.Include(r => r.RaitingSection)
                .Where(r => r.RaitingId == raiting.Id).ToList();
            //Группируем по отрядам
            var groupsraiting = raitingplaces.GroupBy(r => r.SquadId).ToList();
            foreach (var group in groupsraiting)
            {
                //Определяем переменную для баллов
                double mainpoints = 0;
                //Находим записи
                var places = raitingplaces.Where(p => p.SquadId == group.Key).ToList();
                //Идем по каждой записи и считаем баллы, умножив предварительно на коэффициент
                foreach (var place in places)
                {
                    //Умножаем место на коэффициент показателя
                    double count = place.Place * place.RaitingSection.Coef;
                    mainpoints = mainpoints + count;
                }
                //Добавляем в словарик
                maindict.Add(group.Key, mainpoints);
            }
            //Сортируем словарик
            //Сортируем Dictionry по значению баллов от маленького к большому - маленькие выигрывают
            int mainplace = 1;
            var groupmaindict = maindict.OrderBy(pair => pair.Value).ToList();
            var oldmainpoints = groupmaindict[0].Value;
            foreach (var pair in groupmaindict)
            {
                //Текущее количество баллов
                double thismainpoins = pair.Value;
                //Находим текущее место
                if (oldmainpoints != thismainpoins) mainplace++;
                //Находим записи данного отряда в текущем рейтинге
                var squadraitingplaces = raitingplaces.Where(r => r.SquadId == pair.Key).ToList();
                //Для каждой записи данного отряда проставляем главное место
                foreach (var pl in squadraitingplaces)
                {
                    pl.MainPlace = mainplace;
                }
            }
            //Добавляем в рейтинг дату формирования, чтобы определить рейтинг как уже подсчитанный
            raiting.DateofCreation = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }
    }
}
