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
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace StudentSquads.Controllers.API
{
    public class PositionsController : ApiController
    {
        private ApplicationDbContext _context;
        //Для обращения к методам Limit и GetHeadOfStudentSquads
        MembersController memberscontr = new MembersController();
        public PositionsController()
        {
            _context = new ApplicationDbContext();
        }
        [HttpPut]
        public List<HeadsOfStudentSquads> LimitPositions(List<HeadsOfStudentSquads> allheads, HeadsOfStudentSquads headofsquad)
        {
            List<HeadsOfStudentSquads> heads = new List<HeadsOfStudentSquads>();
            if (User.IsInRole("SquadManager"))
            {
                //Для руководителей отрядов ограничиваем отрядом
                heads = allheads.Where(h => h.SquadId == headofsquad.SquadId).ToList();
            }
            else if (User.IsInRole("UniManager"))
            {
                //Для руководителей штаба отбираем только штаб
                heads = allheads.Where(m => m.UniversityHeadquarterId == headofsquad.UniversityHeadquarterId).ToList();
            }
            else if (User.IsInRole("RegionalManager"))
            {
                //Это будет работать только в пределах одного регионального отделения, т.к. ограничения по рег.отделению не стоит
                //Для руководителей рег. отделения ограничиваем одобрением ком.состава
                heads = allheads.Where(m => m.RegionalHeadquarterId == headofsquad.RegionalHeadquarterId).ToList();
            }
            return heads;
        }
        [HttpGet]
        public List<DesignationViewModel> GetPositions()
        {
            List<DesignationViewModel> listpositions = new List<DesignationViewModel>();
            var headofSquads = memberscontr.GetHeadOfStudentSquads();
            var positions = _context.HeadsOfStudentSquads.Include(p => p.Person).Include(p => p.MainPosition)
                .OrderBy(p => p.DateofEnd).ThenByDescending(p => p.DateofBegin)
                .ToList();
            positions = LimitPositions(positions, headofSquads);
            string hasrole = "";
            foreach (var position in positions)
            {
                if (position.HasRole == true) hasrole = "Eсть";
                else hasrole = "Нет";
                DesignationViewModel head = new DesignationViewModel
                {
                    Id = position.Person.Id,
                    FIO = position.Person.FIO,
                    Position = position.Position,
                    DateofBegin = position.DateofBegin?.ToString("dd.MM.yyyy"),
                    DateofEnd = position.DateofEnd?.ToString("dd.MM.yyyy"),
                    HasRoleString = hasrole
                };
                listpositions.Add(head);

            }
            return listpositions;
        }
        [HttpPost]
        public IHttpActionResult CreateNewHead(DesignationViewModel head)
        {
            //Определяем текущего пользоватея
            var headofsquads = memberscontr.GetHeadOfStudentSquads();
            //Определяем, задана ли главная должность
            int? mainposition = null;
            if (head.MainPositionId != 0) { mainposition = head.MainPositionId; }
            //Надо добавить валидацию
            HeadsOfStudentSquads newhead = new HeadsOfStudentSquads
            {
                Id = Guid.NewGuid(),
                PersonId = head.Id,
                Position = head.Position,
                MainPositionId = mainposition,
                //Если не позволять самим подставлять дату начала работы
                DateofBegin = DateTime.Now,
                SquadId = headofsquads.SquadId,
                UniversityHeadquarterId = headofsquads.UniversityHeadquarterId,
                RegionalHeadquarterId = headofsquads.RegionalHeadquarterId
            };
            if (head.HasRole)
            {
                newhead.HasRole = true;
                string role = "";
                if (User.IsInRole("SquadManager")) role = "SquadManager";
                else if (User.IsInRole("UniManager")) role = "UniManager";
                else if (User.IsInRole("RegionalManager")) role = "RegionalManager";
                else if (User.IsInRole("DesantManager")) role = "DesantManager";
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
                //Находим id пользователя, который связан с личностью
                var id = _context.People.SingleOrDefault(i => i.Id == head.Id).ApplicationUserId;
                userManager.AddToRole(id, role);
            }
            else newhead.HasRole = false;
            _context.HeadsOfStudentSquads.Add(newhead);
            _context.SaveChanges();
            return Ok();
        }
        }
}
