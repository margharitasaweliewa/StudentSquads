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
        [HttpPost]
        public IHttpActionResult CreateNewWork(WorkViewModel work)
        {
            foreach (var memberId in work.MembersIds)
            {
                Work newwork = new Work
                {
                    Id = Guid.NewGuid(),
                    MemberId = memberId,
                    EmployerId = work.EmployerId,
                    WorkProjectId = work.WorkProjectId,
                    DateofBegin = (DateTime)work.DateofBegin,
                    DateofEnd = (DateTime)work.DateofEnd,
                Alternative = work.Alternative
                };
                System.Type b = newwork.DateofBegin.GetType();
                _context.Works.Add(newwork);
            }
            _context.SaveChanges();
            return Ok();
        }
    
}
}
