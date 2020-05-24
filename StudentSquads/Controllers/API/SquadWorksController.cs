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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace StudentSquads.Controllers.API
{
    public class SquadWorksController : ApiController
    {
        private ApplicationDbContext _context;
        //Для обращения к методам Limit и GetHeadOfStudentSquads
        public SquadWorksController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        [HttpDelete]
        public IHttpActionResult Rebuke(Guid id, string reason)
        {
            var workInDb = _context.Works.Single(w => w.Id == id);
            //Находим главную запись
            var work = _context.Works.Single(w => w.Id == workInDb.OriginalWorkId);
            //проставляем причину выговора и "Целина засчитана" = false
                work.Rebuke = reason;
                work.Affirmed = false;
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost]
        public IHttpActionResult MakeList()
        {
            //Всем записям текущего сезона проставляем сезон
            var allworks = _context.Works.Where(w => w.Season == null).ToList();
            foreach (var work in allworks) work.Season = DateTime.Now.Year;
            //Засчитываем целину всем, кому не был выписан выговор в текущем сезоне и кто не удален из списка
            var groups= allworks.Where(w => w.Removed == null).ToList() 
            .GroupBy(w => w.OriginalWorkId).ToList();
            foreach (var group in groups)
            {
                var workInDb = _context.Works.Single(w => w.Id == group.Key);
                //Проверяем, что запись была добавлена и что не было выговора
                if (workInDb.Approved == true && workInDb.Affirmed != false) workInDb.Affirmed = true;
            }
            //Сохраняем изменения
            _context.SaveChanges();
            //Теперь находим только основные записи и группируем их по отрядам
            var works = _context.Works.Include(w => w.Member)
                .Where(w => (w.Season == DateTime.Now.Year) && (w.Removed == null) && (w.Id == w.OriginalWorkId)&&(w.Approved == true))
                .ToList();
            var squadgroups = works.GroupBy(w => w.Member.SquadId).ToList();
            //Идем по каждому отряду
            foreach (var squad in squadgroups)
            {
                bool affirmed = false;
                //Считаем кол-во записей
                int maincount = squad.Count();
                //Из них считаем записи, где Alternative = true, это альтернативно
                int alternative = squad.Where(w => w.Alternative == true).Count();
                //Из общего считаем также с засчитанной wелиной - кол-во кирпичиков
                int budges = squad.Where(w => w.Affirmed == true).Count();
                //Считаем кол-во активных членов отряда
                double memberscount = _context.Members
                    .Where(m => (m.DateOfEnter != null) && (m.DateOfExit == null) && (m.SquadId == squad.Key))
                    .Count();
                //Делим кол-во тех, кому засчтена целина, на общее кол-во активных членов
                double coef = budges/memberscount;
                //Если половина и больше, тогда целину засчитываем
                if (coef >= 0.5) affirmed = true;
                //Если целина отряду не засчитана, значки никому не выдаются
                else budges = 0;
                //Добавляем запись в SquadWork с данными данными
                SquadWork squadWork = new SquadWork
                {
                    Id = Guid.NewGuid(),
                    Season = DateTime.Now.Year,
                    Affirmed = affirmed,
                    Count = maincount,
                    AlternativeCount = alternative,
                    BadgesCount = budges,
                    SquadId = squad.Key
                };
                _context.SquadWorks.Add(squadWork);
            }
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet]
        public List<SquadWorkViewModel> AllSquadWorks()
        {
            List<SquadWorkViewModel> listsquadworks = new List<SquadWorkViewModel>();
            var squadworks = _context.SquadWorks.Include(w => w.Squad)
                .OrderByDescending(w => w.Season).ThenBy(w => w.Squad.Name).ToList();
            foreach(var squadwork in squadworks)
            {
                string affirmed = "Не зачтено";
                if (squadwork.Affirmed) affirmed = "Зачтено";
                string uni = _context.Squads.Include(s => s.UniversityHeadquarter)
                    .Single(s => s.Id == squadwork.SquadId).UniversityHeadquarter.University;
                SquadWorkViewModel work = new SquadWorkViewModel
                {
                    Id = squadwork.Id,
                    Season = squadwork.Season.ToString(),
                    Squad = squadwork.Squad.Name,
                    Uni = uni,
                    Count = squadwork.Count,
                    AlternativeCount = squadwork.AlternativeCount,
                    BudgesCount = squadwork.BadgesCount,
                    Affirmed = affirmed
                };
                listsquadworks.Add(work);
            }
            return listsquadworks;
        }

    }
}
